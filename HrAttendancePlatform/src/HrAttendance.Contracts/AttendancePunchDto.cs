namespace HrAttendance.Contracts;

public sealed record AttendancePunchDto(
    Guid Id,
    string EmployeeCode,
    string EmployeeName,
    string DeviceSerialNumber,
    string PunchType,
    string VerificationMode,
    DateTimeOffset PunchUtc);