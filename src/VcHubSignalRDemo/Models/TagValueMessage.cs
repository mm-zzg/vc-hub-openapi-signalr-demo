using System.Text.Json.Serialization;

namespace VcHubSignalRDemo.Models;

public sealed class TagValueMessage
{
    [JsonPropertyName("receivedAt")]
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("value")]
    public dynamic Value { get; set; } = 0;

    [JsonPropertyName("quality")]
    public string Quality { get; set; } = string.Empty;

    [JsonPropertyName("time")]
    public DateTimeOffset? Time { get; set; }

    [JsonPropertyName("rawJson")]
    public string RawJson { get; set; } = string.Empty;
}
