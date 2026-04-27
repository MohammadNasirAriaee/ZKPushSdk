namespace HrAttendance.Contracts;

public sealed record SyncEmployeeToDeviceRequest(
    string EmployeeCode,
    string FullName,
    string Department);
