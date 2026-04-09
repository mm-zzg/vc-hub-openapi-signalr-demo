using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiDemo.Models
{
    public class ShelvedAlarmModel
    {
        [JsonPropertyName("eventId")]
        public string EventId { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("expirationTime")]
        public DateTime ExpirationTime { get; set; }

        [JsonPropertyName("operator")]
        public string Operator { get; set; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
