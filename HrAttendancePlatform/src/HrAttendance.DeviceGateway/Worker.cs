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

    public Worker(
        ILogger<Worker> logger,
        IOptions<DeviceGatewayOptions> options,
        IHrAttendancePlatformService platformService)
    {
        _logger = logger;
        _options = options.Value;
        _platformService = platformService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var dashboard = _platformService.GetDashboardSummary();

        _logger.LogInformation(
            "Device gateway ready on {Host}:{Port} using {Protocol}. Employees seeded: {EmployeeCount}. Active devices seeded: {ActiveDevices}. Vendor listener source: {ListenerSource}",
            _options.ListenHost,
            _options.ListenPort,
            _options.Protocol,
            dashboard.TotalEmployees,
            dashboard.ActiveDevices,
            _options.VendorListenerSource);

        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Gateway heartbeat at {UtcNow}", DateTimeOffset.UtcNow);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
