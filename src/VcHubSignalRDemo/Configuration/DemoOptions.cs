namespace VcHubSignalRDemo.Configuration;

public sealed class DemoOptions
{
    public int MaxMessagesPerStream { get; set; } = 200;

    public List<string> DefaultTagPaths { get; set; } = ["Default:m1"];
}
