using HrAttendance.Application;
using HrAttendance.Contracts;
using HrAttendance.Domain;

namespace HrAttendance.Infrastructure;

public sealed class InMemoryHrAttendancePlatformService : IHrAttendancePlatformService
{
    private readonly IReadOnlyCollection<Employee> _employees;
    private readonly IReadOnlyCollection<AttendanceDevice> _devices;
    private readonly IReadOnlyCollection<AttendancePunch> _punches;

    public InMemoryHrAttendancePlatformService()
    {
        var now = DateTimeOffset.UtcNow;

        _employees =
        [
            new Employee
            {
                Id = Guid.Parse("A53202A8-C6EF-4958-8D33-F32B42E20901"),
                EmployeeCode = "EMP-1001",
                FullName = "Aisha Khan",
                Department = "Operations",
                Branch = "Head Office",
                EmploymentStatus = "Active"
            },
            new Employee
            {
                Id = Guid.Parse("20B3C1F5-C451-47B0-B4C5-A275C3CB6E7C"),
                EmployeeCode = "EMP-1002",
                FullName = "Rahim Uddin",
                Department = "Finance",
                Branch = "Head Office",
                EmploymentStatus = "Active"
            },
            new Employee
            {
                Id = Guid.Parse("CF34D5F4-4DAB-4A51-B171-8A4555F88A10"),
                EmployeeCode = "EMP-1003",
                FullName = "Fatima Noor",
                Department = "Human Resources",
                Branch = "Factory 01",
                EmploymentStatus = "Probation"
            }
        ];

        _devices =
        [
            new AttendanceDevice
            {
                Id = Guid.Parse("759F8E7A-8681-4B85-8A78-6F9FF9AD3F2A"),
                SerialNumber = "ZK-IFACE-001",
                Name = "Front Gate Terminal",
                Model = "iFace 950",
                Location = "Head Office",
                Protocol = "ZKTeco Push/ADMS",
                Status = "Online",
                LastSeenUtc = now.AddMinutes(-2)
            },
            new AttendanceDevice
            {
                Id = Guid.Parse("4F712FC2-6A75-43AE-8471-D0A30B79C532"),
                SerialNumber = "ZK-ISPEED-002",
                Name = "Production Entrance",
                Model = "iSpeedFace V5L",
                Location = "Factory 01",
                Protocol = "ZKTeco Push/ADMS",
                Status = "Online",
                LastSeenUtc = now.AddMinutes(-5)
            },
            new AttendanceDevice
            {
                Id = Guid.Parse("8424294D-9A5C-43D6-9D10-43308E006799"),
                SerialNumber = "ZK-K40-003",
                Name = "Warehouse Door",
                Model = "K40 Pro",
                Location = "Warehouse",
                Protocol = "ZKTeco Push/ADMS",
                Status = "Offline",
                LastSeenUtc = now.AddHours(-6)
            }
        ];

        _punches =
        [
            new AttendancePunch
            {
                Id = Guid.Parse("C1C93E63-D01A-4879-B9B4-2D601F115B60"),
                EmployeeCode = "EMP-1001",
                EmployeeName = "Aisha Khan",
                DeviceSerialNumber = "ZK-IFACE-001",
                PunchType = "CheckIn",
                VerificationMode = "Face",
                PunchUtc = now.AddMinutes(-30)
            },
            new AttendancePunch
            {
                Id = Guid.Parse("52B98E45-8403-40CC-97F7-63D9995C08AA"),
                EmployeeCode = "EMP-1002",
                EmployeeName = "Rahim Uddin",
                DeviceSerialNumber = "ZK-ISPEED-002",
                PunchType = "CheckIn",
                VerificationMode = "Fingerprint",
                PunchUtc = now.AddMinutes(-25)
            },
            new AttendancePunch
            {
                Id = Guid.Parse("E870A93B-D05E-4D94-94E2-742484EE9356"),
                EmployeeCode = "EMP-1003",
                EmployeeName = "Fatima Noor",
                DeviceSerialNumber = "ZK-IFACE-001",
                PunchType = "BreakOut",
                VerificationMode = "Face",
                PunchUtc = now.AddMinutes(-10)
            }
        ];
    }

    public DashboardSummaryDto GetDashboardSummary()
    {
        return new DashboardSummaryDto(
            TotalEmployees: _employees.Count,
            ActiveDevices: _devices.Count(device => device.Status == "Online"),
            TodayPunches: _punches.Count(punch => punch.PunchUtc.Date == DateTimeOffset.UtcNow.Date),
            PendingExceptions: 1,
            LastDeviceSyncUtc: _devices.Max(device => device.LastSeenUtc));
    }

    public IReadOnlyCollection<EmployeeSummaryDto> GetEmployees()
    {
        return _employees
            .Select(employee => new EmployeeSummaryDto(
                employee.Id,
                employee.EmployeeCode,
                employee.FullName,
                employee.Department,
                employee.Branch,
                employee.EmploymentStatus))
            .ToArray();
    }

    public IReadOnlyCollection<DeviceSummaryDto> GetDevices()
    {
        return _devices
            .Select(device => new DeviceSummaryDto(
                device.Id,
                device.SerialNumber,
                device.Name,
                device.Model,
                device.Location,
                device.Protocol,
                device.Status,
                device.LastSeenUtc))
            .ToArray();
    }

    public IReadOnlyCollection<AttendancePunchDto> GetRecentPunches()
    {
        return _punches
            .OrderByDescending(punch => punch.PunchUtc)
            .Select(punch => new AttendancePunchDto(
                punch.Id,
                punch.EmployeeCode,
                punch.EmployeeName,
                punch.DeviceSerialNumber,
                punch.PunchType,
                punch.VerificationMode,
                punch.PunchUtc))
            .ToArray();
    }
}