using HrAttendance.Application;
using HrAttendance.Domain;

namespace HrAttendance.Infrastructure;

/// <summary>
/// In-memory store for punches received from device push events.
/// Replace with EF Core implementation when adding a real database.
/// </summary>
public sealed class InMemoryAttendancePushService : IAttendancePushService
{
    private readonly List<AttendancePunch> _punches = [];

    public InMemoryAttendancePushService() { }

    public Task RecordPunchAsync(string deviceSerialNumber, string employeeCode, DateTime punchTime, string punchType, string verifyMode)
    {
        var punch = new AttendancePunch
        {
            Id = Guid.NewGuid(),
            EmployeeCode = employeeCode,
            EmployeeName = employeeCode,
            DeviceSerialNumber = deviceSerialNumber,
            PunchType = punchType,
            VerificationMode = verifyMode,
            PunchUtc = new DateTimeOffset(punchTime, TimeSpan.Zero)
        };

        lock (_punches)
        {
            _punches.Add(punch);
        }

        Console.WriteLine($"[Punch] Employee={employeeCode}, Device={deviceSerialNumber}, Type={punchType}, Mode={verifyMode}, Time={punchTime}");
        return Task.CompletedTask;
    }

    public Task RecordOperationLogAsync(string deviceSerialNumber, string operationType, string operatorCode, DateTime opTime)
    {
        Console.WriteLine($"[OpLog] Device={deviceSerialNumber}, Type={operationType}, Operator={operatorCode}, Time={opTime}");
        return Task.CompletedTask;
    }

    public IReadOnlyList<AttendancePunch> GetAllPunches()
    {
        lock (_punches)
        {
            return _punches.AsReadOnly();
        }
    }
}
