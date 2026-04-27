using HrAttendance.DeviceGateway;
using HrAttendance.Application;
using HrAttendance.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.Configure<DeviceGatewayOptions>(builder.Configuration.GetSection(DeviceGatewayOptions.SectionName));
builder.Services.AddSingleton<IHrAttendancePlatformService, InMemoryHrAttendancePlatformService>();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
