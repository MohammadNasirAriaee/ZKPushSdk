using HrAttendance.Application;
using HrAttendance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IHrAttendancePlatformService, InMemoryHrAttendancePlatformService>();

var app = builder.Build();

app.UseHttpsRedirection();

var api = app.MapGroup("/api");

api.MapGet("/health", () => Results.Ok(new
{
    status = "Healthy",
    service = "HrAttendance.Api",
    utcNow = DateTimeOffset.UtcNow
}))
.WithName("GetHealth");

api.MapGet("/dashboard", (IHrAttendancePlatformService service) => Results.Ok(service.GetDashboardSummary()))
    .WithName("GetDashboardSummary");

api.MapGet("/employees", (IHrAttendancePlatformService service) => Results.Ok(service.GetEmployees()))
    .WithName("GetEmployees");

api.MapGet("/devices", (IHrAttendancePlatformService service) => Results.Ok(service.GetDevices()))
    .WithName("GetDevices");

api.MapGet("/attendance/punches", (IHrAttendancePlatformService service) => Results.Ok(service.GetRecentPunches()))
    .WithName("GetRecentPunches");

app.Run();
