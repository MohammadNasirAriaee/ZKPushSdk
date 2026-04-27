namespace HrAttendance.DeviceGateway;

public sealed class DeviceGatewayOptions
{
    public const string SectionName = "DeviceGateway";

    public string ListenHost { get; init; } = "0.0.0.0";

    public int ListenPort { get; init; } = 8080;

    public string Protocol { get; init; } = "ZKTeco Push/ADMS";

    public string VendorListenerSource { get; init; } = "..\\Attendance\\ListenClient.cs";
}