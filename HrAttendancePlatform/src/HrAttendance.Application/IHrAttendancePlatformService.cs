using HrAttendance.Contracts;

namespace HrAttendance.Application;

public interface IHrAttendancePlatformService
{
    DashboardSummaryDto GetDashboardSummary();

    IReadOnlyCollection<EmployeeSummaryDto> GetEmployees();

    IReadOnlyCollection<DeviceSummaryDto> GetDevices();

    IReadOnlyCollection<AttendancePunchDto> GetRecentPunches();
}