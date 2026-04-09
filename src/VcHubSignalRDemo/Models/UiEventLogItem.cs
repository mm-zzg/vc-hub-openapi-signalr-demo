namespace VcHubSignalRDemo.Models;

public sealed class UiEventLogItem
{
    public DateTimeOffset Time { get; set; } = DateTimeOffset.UtcNow;

    public string Level { get; set; } = "Info";

    public string Message { get; set; } = string.Empty;
}
