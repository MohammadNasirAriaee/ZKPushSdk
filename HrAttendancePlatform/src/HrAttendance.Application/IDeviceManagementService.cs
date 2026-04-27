using HrAttendance.Contracts;
using HrAttendance.Domain;

namespace HrAttendance.Application;

public interface IDeviceManagementService
{
    Task<Device> RegisterDeviceAsync(RegisterDeviceRequest request);

    Task<Device?> GetDeviceBySerialNumberAsync(string serialNumber);

    Task<IReadOnlyCollection<Device>> GetAllDevicesAsync();

    Task UpdateDeviceHeartbeatAsync(string serialNumber);

    Task UpdateDeviceStatusAsync(string serialNumber, string status);

    Task QueueCommandForDeviceAsync(string serialNumber, string commandType, string commandContent);

    Task<IReadOnlyCollection<DeviceCommand>> GetPendingCommandsAsync(string serialNumber);

    Task MarkCommandExecutedAsync(Guid commandId, string result);
}
