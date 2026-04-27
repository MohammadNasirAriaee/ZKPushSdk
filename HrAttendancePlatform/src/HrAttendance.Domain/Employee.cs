namespace HrAttendance.Domain;

public sealed class Employee
{
    public required Guid Id { get; init; }

    public required string EmployeeCode { get; init; }

    public required string FullName { get; init; }

    public required string Department { get; init; }

    public required string Branch { get; init; }

    public required string EmploymentStatus { get; init; }
}