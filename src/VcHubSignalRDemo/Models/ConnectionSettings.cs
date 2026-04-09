namespace VcHubSignalRDemo.Models;

public sealed class ConnectionSettings
{
    public string Url { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public string ClientSecret { get; set; } = string.Empty;

    public string PrimaryRoute { get; set; } = "ws/v/realtimedata";
}
