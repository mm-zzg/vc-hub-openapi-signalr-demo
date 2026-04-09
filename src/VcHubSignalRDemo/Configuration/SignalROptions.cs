namespace VcHubSignalRDemo.Configuration;

public sealed class SignalROptions
{
    public string PrimaryRoute { get; set; } = "ws/v/realtimedata";

    public bool EnableCertificateValidationBypassForDemo { get; set; } = true;
}
