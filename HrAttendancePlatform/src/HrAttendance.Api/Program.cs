using HrAttendance.Application;
using HrAttendance.Contracts;
using HrAttendance.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IHrAttendancePlatformService, InMemoryHrAttendancePlatformService>();
builder.Services.AddSingleton<IDeviceManagementService, InMemoryDeviceManagementService>();
builder.Services.AddSingleton<IAttendancePushService, InMemoryAttendancePushService>();

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

api.MapPost("/devices/register", async (RegisterDeviceRequest request, IDeviceManagementService service) =>
{
    var device = await service.RegisterDeviceAsync(request);
    return Results.Created($"/api/devices/{device.Id}", device);
})
    .WithName("RegisterDevice");

api.MapGet("/devices/all", async (IDeviceManagementService service) =>
{
    var devices = await service.GetAllDevicesAsync();
    return Results.Ok(devices);
})
    .WithName("GetAllDevices");

api.MapGet("/devices/{serialNumber}", async (string serialNumber, IDeviceManagementService service) =>
{
    var device = await service.GetDeviceBySerialNumberAsync(serialNumber);
    if (device is null)
        return Results.NotFound();
    return Results.Ok(device);
})
    .WithName("GetDeviceBySerialNumber");

// Sync an employee to a device by queuing a DATA UPDATE USER command.
// The device will receive this on its next getrequest poll (every ~20 seconds).
api.MapPost("/devices/{serialNumber}/sync-employee", async (
    string serialNumber,
    SyncEmployeeToDeviceRequest request,
    IDeviceManagementService service) =>
{
    var device = await service.GetDeviceBySerialNumberAsync(serialNumber);
    if (device is null)
        return Results.NotFound($"Device {serialNumber} not found");

    // ZKTeco ADMS command format for adding/updating a user
    // CMD=DATA UPDATE USER PIN=<id>\tName=<name>\tPri=0\tPasswd=\tCard=\tGrp=1\tTZ=0000000100000000\tVerify=0
    string commandContent =
        $"DATA UPDATE USER PIN={request.EmployeeCode}\t" +
        $"Name={request.FullName}\t" +
        $"Pri=0\tPasswd=\tCard=\tGrp=1\t" +
        $"TZ=0000000100000000\tVerify=0";

    await service.QueueCommandForDeviceAsync(serialNumber, "DATA UPDATE USER", commandContent);

    return Results.Accepted($"/api/devices/{serialNumber}", new
    {
        message = $"Employee {request.EmployeeCode} ({request.FullName}) queued for sync to device {serialNumber}",
        note = "Device will receive this command on its next getrequest poll (within ~20 seconds)"
    });
})
    .WithName("SyncEmployeeToDevice");

// Delete a user from a device
api.MapDelete("/devices/{serialNumber}/users/{employeeCode}", async (
    string serialNumber,
    string employeeCode,
    IDeviceManagementService service) =>
{
    var device = await service.GetDeviceBySerialNumberAsync(serialNumber);
    if (device is null)
        return Results.NotFound($"Device {serialNumber} not found");

    string commandContent = $"DATA DELETE USER PIN={employeeCode}";
    await service.QueueCommandForDeviceAsync(serialNumber, "DATA DELETE USER", commandContent);

    return Results.Accepted($"/api/devices/{serialNumber}", new
    {
        message = $"Delete employee {employeeCode} from device {serialNumber} queued"
    });
})
    .WithName("DeleteUserFromDevice");

// Reboot a device remotely
api.MapPost("/devices/{serialNumber}/reboot", async (
    string serialNumber,
    IDeviceManagementService service) =>
{
    var device = await service.GetDeviceBySerialNumberAsync(serialNumber);
    if (device is null)
        return Results.NotFound($"Device {serialNumber} not found");

    await service.QueueCommandForDeviceAsync(serialNumber, "REBOOT", "REBOOT");

    return Results.Accepted($"/api/devices/{serialNumber}", new
    {
        message = $"Reboot command queued for device {serialNumber}"
    });
})
    .WithName("RebootDevice");

app.Run();
