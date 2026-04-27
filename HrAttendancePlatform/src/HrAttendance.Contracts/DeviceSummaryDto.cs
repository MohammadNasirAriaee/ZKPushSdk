namespace HrAttendance.Contracts;

public sealed record DeviceSummaryDto(
    Guid Id,
    string SerialNumber,
    string Name,
    string Model,
    string Location,
    string Protocol,
    string Status,
    DateTimeOffset LastSeenUtc);