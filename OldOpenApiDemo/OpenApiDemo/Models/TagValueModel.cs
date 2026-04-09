using OpenApiDemo.Common;
using System.Text.Json.Serialization;

namespace OpenApiDemo.Models
{
    public class TagValueModel
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("value")]
        [JsonConverter(typeof(ObjectConverter))]
        public dynamic Value { get; set; }

        [JsonPropertyName("time")]
        public DateTime Time { get; set; }

        [JsonPropertyName("quality")]
        public string Quality { get; set; }

    }
}
