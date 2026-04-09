using System.Text.Json.Serialization;

namespace VcHubSignalRDemo.Models;

public sealed class AlarmMessage
{
    [JsonPropertyName("receivedAt")]
    public DateTimeOffset ReceivedAt { get; set; } = DateTimeOffset.UtcNow;

    [JsonPropertyName("eventId")]
    public string EventId { get; set; } = string.Empty;

    [JsonPropertyName("path")]
    public string Path { get; set; } = string.Empty;

    [JsonPropertyName("priority")]
    public string Priority { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;

    [JsonPropertyName("eventTime")]
    public DateTimeOffset? EventTime { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("rawJson")]
    public string RawJson { get; set; } = string.Empty;
}
