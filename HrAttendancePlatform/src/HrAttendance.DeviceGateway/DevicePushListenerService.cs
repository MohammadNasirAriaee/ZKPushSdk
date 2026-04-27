using HrAttendance.Application;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Text;

namespace HrAttendance.DeviceGateway;

public sealed class DevicePushListenerService : IDisposable
{
    private readonly HttpListener _listener;
    private readonly IDeviceManagementService _deviceService;
    private readonly IAttendancePushService _attendanceService;
    private readonly ILogger<DevicePushListenerService> _logger;
    private CancellationTokenSource? _cts;

    public DevicePushListenerService(
        IDeviceManagementService deviceService,
        IAttendancePushService attendanceService,
        ILogger<DevicePushListenerService> logger)
    {
        _deviceService = deviceService;
        _attendanceService = attendanceService;
        _logger = logger;
        _listener = new HttpListener();
    }

    public async Task StartAsync(string prefix, CancellationToken stoppingToken)
    {
        try
        {
            _listener.Prefixes.Add(prefix);
            _listener.Start();
            _logger.LogInformation("Device push listener started on {Prefix}", prefix);

            _cts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var context = await _listener.GetContextAsync();
                    _ = HandleRequestAsync(context, stoppingToken);
                }
                catch (HttpListenerException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error accepting listener context");
                }
            }
        }
        finally
        {
            Stop();
        }
    }

    private async Task HandleRequestAsync(HttpListenerContext context, CancellationToken cancellationToken)
    {
        try
        {
            var request = context.Request;
            var response = context.Response;

            _logger.LogInformation("Device request: {Method} {Url}", request.HttpMethod, request.Url?.PathAndQuery);

            string requestBody = "";
            if (request.HasEntityBody)
            {
                using (var reader = new StreamReader(request.InputStream, Encoding.UTF8))
                {
                    requestBody = await reader.ReadToEndAsync();
                }
            }

            string responseText = "OK";
            string responseCode = "200 OK";

            if (request.RawUrl?.Contains("cdata?") == true)
            {
                var sn = ExtractParameter(request.RawUrl, "SN");
                if (string.IsNullOrEmpty(sn))
                {
                    responseCode = "400 Bad Request";
                    responseText = "Missing SN";
                }
                else if (request.HttpMethod == "GET")
                {
                    // Device first contact - requesting its configuration
                    await _deviceService.UpdateDeviceHeartbeatAsync(sn);
                    responseText = BuildDeviceInitResponse(sn);
                    _logger.LogInformation("Device {SN} initial connection - sent config", sn);
                }
                else if (request.HttpMethod == "POST")
                {
                    await _deviceService.UpdateDeviceHeartbeatAsync(sn);
                    await ParseAndStorePostDataAsync(sn, requestBody);
                }
            }
            else if (request.RawUrl?.Contains("getrequest?") == true)
            {
                var sn = ExtractParameter(request.RawUrl, "SN");
                if (!string.IsNullOrEmpty(sn))
                {
                    await _deviceService.UpdateDeviceHeartbeatAsync(sn);
                    var commands = await _deviceService.GetPendingCommandsAsync(sn);
                    if (commands.Count > 0)
                    {
                        responseText = string.Join("\r\n", commands.Select(c => c.CommandContent));
                    }
                    _logger.LogInformation("Device {SN} polled for commands, found {Count}", sn, commands.Count);
                }
            }
            else if (request.RawUrl?.Contains("ping?") == true)
            {
                var sn = ExtractParameter(request.RawUrl, "SN");
                if (!string.IsNullOrEmpty(sn))
                {
                    await _deviceService.UpdateDeviceHeartbeatAsync(sn);
                    _logger.LogInformation("Device {SN} ping received", sn);
                }
            }
            else
            {
                responseCode = "400 Bad Request";
                responseText = "Unknown command";
                _logger.LogWarning("Unknown device request: {Url}", request.RawUrl);
            }

            response.StatusCode = 200;
            response.ContentType = "text/plain";
            response.ContentEncoding = Encoding.UTF8;

            var responseBytes = Encoding.UTF8.GetBytes($"HTTP/1.1 {responseCode}\r\nContent-Type: text/plain\r\nContent-Length: {responseText.Length}\r\n\r\n{responseText}");
            response.OutputStream.Write(responseBytes, 0, responseBytes.Length);
            response.OutputStream.Close();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling device request");
            try
            {
                context.Response.StatusCode = 500;
                context.Response.Close();
            }
            catch { }
        }
    }

    private static string BuildDeviceInitResponse(string sn)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"GET OPTION FROM:{sn}");
        sb.AppendLine("Stamp=9999");
        sb.AppendLine("OpStamp=9999");
        sb.AppendLine("PhotoStamp=9999");
        sb.AppendLine("TransFlag=111100000000");
        sb.AppendLine("Realtime=1");
        sb.AppendLine("Delay=20");
        sb.AppendLine("TransInterval=1");
        sb.AppendLine("TransTimes=00:00;14:05");
        sb.AppendLine("TimeZone=+05:00");
        sb.AppendLine("SyncTime=0");
        sb.AppendLine("ServerName=HrAttendancePlatform");
        return sb.ToString();
    }

    private async Task ParseAndStorePostDataAsync(string sn, string body)
    {
        if (string.IsNullOrWhiteSpace(body))
            return;

        // Separate HTTP headers from body (body starts after blank line)
        int bodyStart = body.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        string dataStr = bodyStart >= 0 ? body.Substring(bodyStart + 4) : body;

        if (dataStr.Contains("table=ATTLOG") || dataStr.Contains("Stamp="))
        {
            await ParseAttLogAsync(sn, dataStr);
        }
        else if (dataStr.Contains("table=OPERLOG"))
        {
            await ParseOperLogAsync(sn, dataStr);
        }
        else if (dataStr.Contains("table=options"))
        {
            _logger.LogInformation("Device {SN} pushed options config", sn);
        }
    }

    private async Task ParseAttLogAsync(string sn, string dataStr)
    {
        // Each line: PIN\tDateTime\tStatus\tVerify\tWorkCode\t...
        // e.g.  1234\t2026-04-27 08:30:00\t0\t1\t0
        var lines = dataStr.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        int saved = 0;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("table=") || trimmed.StartsWith("Stamp="))
                continue;

            var fields = trimmed.Split('\t');
            if (fields.Length < 4)
                continue;

            try
            {
                string pin = fields[0].Trim();
                if (!DateTime.TryParse(fields[1].Trim(), out var punchTime))
                    continue;

                // Status 0=Check-In 1=Check-Out 2=Break-Out 3=Break-In 4=OT-In 5=OT-Out
                string status = fields[2].Trim();
                string punchType = status switch
                {
                    "0" => "CheckIn",
                    "1" => "CheckOut",
                    "2" => "BreakOut",
                    "3" => "BreakIn",
                    "4" => "OvertimeIn",
                    "5" => "OvertimeOut",
                    _ => "Unknown"
                };

                // Verify 1=Fingerprint 2=Password 4=Card 15=Face
                string verify = fields[3].Trim();
                string verifyMode = verify switch
                {
                    "1" => "Fingerprint",
                    "2" => "Password",
                    "4" => "Card",
                    "15" => "Face",
                    "200" => "Face",
                    _ => "Other"
                };

                await _attendanceService.RecordPunchAsync(sn, pin, punchTime, punchType, verifyMode);
                saved++;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse ATTLOG line: {Line}", trimmed);
            }
        }

        if (saved > 0)
            _logger.LogInformation("Saved {Count} punches from device {SN}", saved, sn);
    }

    private async Task ParseOperLogAsync(string sn, string dataStr)
    {
        var lines = dataStr.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var trimmed = line.Trim();
            if (string.IsNullOrEmpty(trimmed) || !trimmed.Contains("OPLOG"))
                continue;

            var fields = trimmed.Split('\t');
            if (fields.Length < 3)
                continue;

            try
            {
                // OPLOG OpType=0\tAdmin\t2026-04-27 09:00:00\t...
                string opType = fields[0].Replace("OPLOG OpType=", "").Trim();
                string opOperator = fields.Length > 1 ? fields[1].Trim() : "unknown";
                DateTime.TryParse(fields.Length > 2 ? fields[2].Trim() : "", out var opTime);
                await _attendanceService.RecordOperationLogAsync(sn, opType, opOperator, opTime);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse OPERLOG line: {Line}", trimmed);
            }
        }
    }

    private static string? ExtractParameter(string url, string paramName)
    {
        var parts = url.Split('?', '&');
        foreach (var part in parts)
        {
            if (part.StartsWith($"{paramName}="))
            {
                return part.Substring(paramName.Length + 1);
            }
        }
        return null;
    }

    public void Stop()
    {
        try
        {
            _listener.Stop();
            _listener.Close();
        }
        catch { }
    }

    public void Dispose()
    {
        Stop();
        _cts?.Dispose();
    }
}
