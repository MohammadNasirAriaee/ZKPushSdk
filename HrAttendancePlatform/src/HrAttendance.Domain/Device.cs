namespace HrAttendance.Domain;

public sealed class Device
{
    public required Guid Id { get; init; }

    public required string SerialNumber { get; init; }

    public required string Name { get; init; }

    public required string Model { get; init; }

    public required string Location { get; init; }

    public required string IpAddress { get; init; }

    public required int Port { get; init; }

    public required string Protocol { get; init; } = "ZKTeco Push/ADMS";

    public required string Status { get; set; } = "Offline";

    public DateTimeOffset LastRegisteredUtc { get; set; }

    public DateTimeOffset LastHeartbeatUtc { get; set; }

    public string? FirmwareVersion { get; set; }

    public int UserCount { get; set; }

    public int AttendanceCount { get; set; }
}
