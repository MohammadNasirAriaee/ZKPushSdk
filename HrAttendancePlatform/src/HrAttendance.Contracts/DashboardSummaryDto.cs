namespace HrAttendance.Contracts;

public sealed record DashboardSummaryDto(
    int TotalEmployees,
    int ActiveDevices,
    int TodayPunches,
    int PendingExceptions,
    DateTimeOffset LastDeviceSyncUtc);