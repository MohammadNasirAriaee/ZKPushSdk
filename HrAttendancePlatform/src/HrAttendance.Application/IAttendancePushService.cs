namespace HrAttendance.Application;

/// <summary>
/// Receives parsed attendance events from device push data and stores them.
/// </summary>
public interface IAttendancePushService
{
    Task RecordPunchAsync(string deviceSerialNumber, string employeeCode, DateTime punchTime, string punchType, string verifyMode);

    Task RecordOperationLogAsync(string deviceSerialNumber, string operationType, string operatorCode, DateTime opTime);
}
