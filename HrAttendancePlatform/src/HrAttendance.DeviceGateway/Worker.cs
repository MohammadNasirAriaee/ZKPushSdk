using HrAttendance.Application;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HrAttendance.DeviceGateway;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DeviceGatewayOptions _options;
    private readonly IHrAttendancePlatformService _platformService;
    private readonly DevicePushListenerService _listenerService;

    public Worker(
        ILogger<Worker> logger,
        IOptions<DeviceGatewayOptions> options,
        IHrAttendancePlatformService platformService,
        DevicePushListenerService listenerService)
    {
        _logger = logger;
        _options = options.Value;
        _platformService = platformService;
        _listenerService = listenerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var dashboard = _platformService.GetDashboardSummary();

        _logger.LogInformation(
            "Device gateway starting on {Host}:{Port} using {Protocol}. Employees seeded: {EmployeeCount}. Active devices seeded: {ActiveDevices}.",
            _options.ListenHost,
            _options.ListenPort,
            _options.Protocol,
            dashboard.TotalEmployees,
            dashboard.ActiveDevices);

        var prefix = $"http://{_options.ListenHost}:{_options.ListenPort}/";

        try
        {
            await _listenerService.StartAsync(prefix, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Device listener error");
            throw;
        }
        finally
        {
            _listenerService.Dispose();
        }
    }
}
