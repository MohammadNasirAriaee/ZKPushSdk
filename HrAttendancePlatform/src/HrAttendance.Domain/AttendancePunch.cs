namespace HrAttendance.Domain;

public sealed class AttendancePunch
{
    public required Guid Id { get; init; }

    public required string EmployeeCode { get; init; }

    public required string EmployeeName { get; init; }

    public required string DeviceSerialNumber { get; init; }

    public required string PunchType { get; init; }

    public required string VerificationMode { get; init; }

    public DateTimeOffset PunchUtc { get; init; }
}