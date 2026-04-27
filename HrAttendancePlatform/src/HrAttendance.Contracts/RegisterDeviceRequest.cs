namespace HrAttendance.Contracts;

public sealed record RegisterDeviceRequest(
    string SerialNumber,
    string Name,
    string Model,
    string Location,
    string IpAddress,
    int Port = 8080,
    string? FirmwareVersion = null);
