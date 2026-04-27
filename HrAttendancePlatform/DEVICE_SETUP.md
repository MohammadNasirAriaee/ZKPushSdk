# ZKTeco Device Integration Guide

This guide shows how to configure your iSpeedFace V5L device to push attendance data to this platform and manage employees.

## Prerequisites

- iSpeedFace V5L device at IP `192.168.200.22`
- This server at IP `192.168.150.38`
- Device connected to the same network
- Device web interface accessible (default port 80 or 443)

## Step 1: Configure Device to Push to This Platform

### Option A: Via Device Web Interface

1. Open browser and go to `http://192.168.200.22`
2. Login (default: admin / admin)
3. Navigate to **Settings → Cloud / ADMS / Push Server**
4. Fill in the following:
   - **Server IP**: `192.168.150.38`
   - **Server Port**: `8080`
   - **Protocol**: Select **ADMS** or **HTTP Push**
   - **Enable**: Check the box
5. Click **Test Connection** (should show success)
6. Click **Save** and wait for device to restart

### Option B: Via ZKTeco Web Software

If device is already added to ZKTeco web software:

1. Open ZKTeco web software
2. Right-click device → **Settings**
3. In **Advanced Settings**, find **Server Configuration**
4. Change:
   - Server IP: `192.168.150.38`
   - Server Port: `8080`
5. Save and reboot device

## Step 2: Verify Device Connection

Once device is configured, start both services:

**Terminal 1 (API):**
```powershell
dotnet run --project .\HrAttendancePlatform\src\HrAttendance.Api\HrAttendance.Api.csproj
```

**Terminal 2 (Gateway):**
```powershell
dotnet run --project .\HrAttendancePlatform\src\HrAttendance.DeviceGateway\HrAttendance.DeviceGateway.csproj
```

### Expected Gateway Logs

```
info: HrAttendance.DeviceGateway.Worker[0]
      Device gateway starting on 0.0.0.0:8080 using ZKTeco Push/ADMS.
      
info: HrAttendance.DeviceGateway.DevicePushListenerService[0]
      Device push listener started on http://0.0.0.0:8080/
      
info: HrAttendance.DeviceGateway.DevicePushListenerService[0]
      Device ZK-ISPEED-001 ping received

info: HrAttendance.DeviceGateway.DevicePushListenerService[0]
      Device ZK-ISPEED-001 polled for commands, found 0
```

## Step 3: Register Device in Platform

Once you see ping messages in the gateway logs, the device is connected. Now register it in the system:

```powershell
$deviceRequest = @{
    SerialNumber = "ZK-ISPEED-001"
    Name = "Main Entrance"
    Model = "iSpeedFace V5L"
    Location = "Head Office"
    IpAddress = "192.168.200.22"
    Port = 8080
    FirmwareVersion = "latest"
} | ConvertTo-Json

curl -X POST http://localhost:5123/api/devices/register `
  -ContentType "application/json" `
  -Body $deviceRequest
```

**Expected Response:**
```json
{
  "id": "4f712fc2-6a75-43ae-8471-d0a30b79c533",
  "serialNumber": "ZK-ISPEED-001",
  "name": "Main Entrance",
  "status": "Registered",
  "lastHeartbeatUtc": "2026-04-27T...",
  ...
}
```

## Step 4: Add Employees to Device

The device needs employees to be synced. For now, employees are added via manual HTTP requests (later, a UI will do this).

To add an employee to the device, you would send a command to the device via the API. This is queued and the device polls for commands.

## Step 5: View Device Status

### Get all devices and their status:
```powershell
curl http://localhost:5123/api/devices/all
```

### Get specific device:
```powershell
curl http://localhost:5123/api/devices/ZK-ISPEED-001
```

### Get dashboard (includes active device count):
```powershell
curl http://localhost:5123/api/dashboard
```

## Troubleshooting

### Device not connecting

**Check gateway logs for errors:**
- If you see no ping messages, device is not reaching the platform.
- Verify firewall allows port 8080 on `192.168.150.38`.
- Check device web interface → **Logs** to see connection attempts.

**Verify network connectivity:**
```powershell
# From device machine, test connectivity
ping 192.168.150.38
Test-NetConnection -ComputerName 192.168.150.38 -Port 8080
```

**Check device configuration:**
- Re-verify server IP and port in device settings are correct.
- Ensure device has uploaded new settings (check for "Synchronizing..." message).

### Device connects but no data flowing

- Check device menu → **Test** or **Sync** button to force a push.
- View gateway logs to see if requests are being received.
- Verify the device has attendance records to upload.

## Next Steps

1. **Add multiple employees to device** (via API endpoints or bulk import).
2. **Set up real database** instead of in-memory storage so data persists.
3. **Build admin UI** to manage devices and employees visually.
4. **Configure timezone and attendance rules** on the device.
5. **Enable biometric data sync** (fingerprint, face templates) if needed.

## API Endpoints Reference

| Method | Endpoint | Purpose |
|--------|----------|---------|
| POST | `/api/devices/register` | Register a new device |
| GET | `/api/devices/all` | Get all registered devices |
| GET | `/api/devices/{serialNumber}` | Get specific device status |
| GET | `/api/health` | Health check |
| GET | `/api/dashboard` | System dashboard |
| GET | `/api/employees` | Get all employees |
| GET | `/api/attendance/punches` | Get recent attendance records |
