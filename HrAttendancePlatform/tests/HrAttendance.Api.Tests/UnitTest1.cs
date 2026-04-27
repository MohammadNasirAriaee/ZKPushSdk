using HrAttendance.Infrastructure;

namespace HrAttendance.Api.Tests;

public class UnitTest1
{
    [Fact]
    public void DashboardSummary_UsesSeededData()
    {
        var service = new InMemoryHrAttendancePlatformService();

        var summary = service.GetDashboardSummary();

        Assert.Equal(3, summary.TotalEmployees);
        Assert.Equal(2, summary.ActiveDevices);
        Assert.True(summary.TodayPunches >= 1);
    }
}