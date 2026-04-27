# HR Attendance Platform

This solution is the clean starting point for your product, separate from the vendor demo SDK.

## Projects

- `src/HrAttendance.Api`: ASP.NET Core API for employees, devices, dashboard, and attendance endpoints.
- `src/HrAttendance.DeviceGateway`: Worker service that will host the ZKTeco push listener and background jobs.
- `src/HrAttendance.Domain`: Core domain entities.
- `src/HrAttendance.Application`: Service contracts used by API, worker, and future clients.
- `src/HrAttendance.Infrastructure`: Initial in-memory implementation used for bootstrapping.
- `src/HrAttendance.Contracts`: DTOs returned to web, desktop, and mobile clients.

## Why this is separate from the SDK demo

The existing `Attendance`, `BLL`, `Dal`, `Model`, and `Utils` projects are vendor demo code. Keep them as a protocol reference and migration source. Your product should evolve in this solution.

## Current status

- API endpoints are available for dashboard, employees, devices, and recent punches.
- Device gateway is configured with ZKTeco Push/ADMS defaults and points to the vendor listener source file for migration reference.
- Infrastructure uses seeded in-memory data so the solution can run immediately.

## Run

From the `HrAttendancePlatform` folder:

```powershell
dotnet restore .\HrAttendancePlatform.slnx
dotnet run --project .\src\HrAttendance.Api\HrAttendance.Api.csproj
dotnet run --project .\src\HrAttendance.DeviceGateway\HrAttendance.DeviceGateway.csproj
```

## Recommended next build steps

1. Replace the in-memory service with EF Core and SQL Server or PostgreSQL.
2. Move the ZKTeco listener logic from `Attendance/ListenClient.cs` into `HrAttendance.DeviceGateway` behind interfaces.
3. Add authentication, roles, companies, branches, shifts, holidays, and attendance rules.
4. Add your preferred OpenAPI or Swagger package after NuGet connectivity is stable.
5. Build your admin web UI against `HrAttendance.Api`.
6. Build Windows desktop and mobile clients against the same API instead of connecting directly to devices.

## Device integration strategy

Use the purchased SDK as the device ingestion layer only.

- Devices should push to the worker service endpoint.
- The worker should parse raw device payloads and store them.
- The API should expose business features to the web, desktop, and mobile apps.
- Keep raw push payloads for debugging firmware differences between models such as iFace and iSpeed.