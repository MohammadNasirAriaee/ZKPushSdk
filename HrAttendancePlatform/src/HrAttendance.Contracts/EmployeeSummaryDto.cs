namespace HrAttendance.Contracts;

public sealed record EmployeeSummaryDto(
    Guid Id,
    string EmployeeCode,
    string FullName,
    string Department,
    string Branch,
    string EmploymentStatus);