namespace HrAttendance.Domain;

public sealed class DeviceCommand
{
    public required Guid Id { get; init; }

    public required string DeviceSerialNumber { get; init; }

    public required string CommandType { get; init; }

    public required string CommandContent { get; init; }

    public required string Status { get; set; } = "Pending";

    public DateTimeOffset CreatedUtc { get; init; }

    public DateTimeOffset? ExecutedUtc { get; set; }

    public string? ExecutionResult { get; set; }
}
