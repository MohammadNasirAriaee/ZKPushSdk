using HrAttendance.Application;
using HrAttendance.Contracts;
using HrAttendance.Domain;

namespace HrAttendance.Infrastructure;

public sealed class InMemoryDeviceManagementService : IDeviceManagementService
{
    private readonly Dictionary<string, Device> _devices = new();
    private readonly Dictionary<Guid, DeviceCommand> _commands = new();

    public InMemoryDeviceManagementService()
    {
        SeedSampleDevices();
    }

    public Task<Device> RegisterDeviceAsync(RegisterDeviceRequest request)
    {
        var device = new Device
        {
            Id = Guid.NewGuid(),
            SerialNumber = request.SerialNumber,
            Name = request.Name,
            Model = request.Model,
            Location = request.Location,
            IpAddress = request.IpAddress,
            Port = request.Port,
            Protocol = "ZKTeco Push/ADMS",
            Status = "Registered",
            LastRegisteredUtc = DateTimeOffset.UtcNow,
            LastHeartbeatUtc = DateTimeOffset.UtcNow,
            FirmwareVersion = request.FirmwareVersion,
            UserCount = 0,
            AttendanceCount = 0
        };

        _devices[device.SerialNumber] = device;
        return Task.FromResult(device);
    }

    public Task<Device?> GetDeviceBySerialNumberAsync(string serialNumber)
    {
        return Task.FromResult(_devices.TryGetValue(serialNumber, out var device) ? device : null);
    }

    public Task<IReadOnlyCollection<Device>> GetAllDevicesAsync()
    {
        return Task.FromResult<IReadOnlyCollection<Device>>(_devices.Values.ToList().AsReadOnly());
    }

    public Task UpdateDeviceHeartbeatAsync(string serialNumber)
    {
        if (_devices.TryGetValue(serialNumber, out var device))
        {
            device.LastHeartbeatUtc = DateTimeOffset.UtcNow;
            device.Status = "Online";
        }

        return Task.CompletedTask;
    }

    public Task UpdateDeviceStatusAsync(string serialNumber, string status)
    {
        if (_devices.TryGetValue(serialNumber, out var device))
        {
            device.Status = status;
            device.LastHeartbeatUtc = DateTimeOffset.UtcNow;
        }

        return Task.CompletedTask;
    }

    public Task QueueCommandForDeviceAsync(string serialNumber, string commandType, string commandContent)
    {
        var command = new DeviceCommand
        {
            Id = Guid.NewGuid(),
            DeviceSerialNumber = serialNumber,
            CommandType = commandType,
            CommandContent = commandContent,
            Status = "Pending",
            CreatedUtc = DateTimeOffset.UtcNow
        };

        _commands[command.Id] = command;
        return Task.CompletedTask;
    }

    public Task<IReadOnlyCollection<DeviceCommand>> GetPendingCommandsAsync(string serialNumber)
    {
        var pending = _commands.Values
            .Where(cmd => cmd.DeviceSerialNumber == serialNumber && cmd.Status == "Pending")
            .ToList()
            .AsReadOnly();

        return Task.FromResult<IReadOnlyCollection<DeviceCommand>>(pending);
    }

    public Task MarkCommandExecutedAsync(Guid commandId, string result)
    {
        if (_commands.TryGetValue(commandId, out var command))
        {
            command.Status = "Executed";
            command.ExecutedUtc = DateTimeOffset.UtcNow;
            command.ExecutionResult = result;
        }

        return Task.CompletedTask;
    }

    private void SeedSampleDevices()
    {
        var now = DateTimeOffset.UtcNow;

        _devices["ZK-ISPEED-001"] = new Device
        {
            Id = Guid.Parse("4F712FC2-6A75-43AE-8471-D0A30B79C533"),
            SerialNumber = "ZK-ISPEED-001",
            Name = "Main Entrance",
            Model = "iSpeedFace V5L",
            Location = "Head Office",
            IpAddress = "192.168.200.22",
            Port = 8080,
            Protocol = "ZKTeco Push/ADMS",
            Status = "Registered",
            LastRegisteredUtc = now.AddMinutes(-10),
            LastHeartbeatUtc = now.AddMinutes(-2),
            FirmwareVersion = "latest",
            UserCount = 0,
            AttendanceCount = 0
        };
    }
}
