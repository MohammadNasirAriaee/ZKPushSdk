namespace HrAttendance.Domain;

public sealed class AttendanceDevice
{
    public required Guid Id { get; init; }

    public required string SerialNumber { get; init; }

    public required string Name { get; init; }

    public required string Model { get; init; }

    public required string Location { get; init; }

    public required string Protocol { get; init; }

    public required string Status { get; init; }

    public DateTimeOffset LastSeenUtc { get; init; }
}