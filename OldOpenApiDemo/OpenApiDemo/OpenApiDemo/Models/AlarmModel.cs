using OpenApiDemo.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiDemo.Models
{
    public class AlarmModel
    {
        [JsonPropertyName("eventId")]

        public string EventId { get; set; }


        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }


        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("priority")]
        public string Priority { get; set; }



        [JsonPropertyName("eventTime")]
        public DateTime EventTime { get; set; }


        [JsonPropertyName("status")]
        public string Status { get; set; }


        [JsonPropertyName("value")]
        [JsonConverter(typeof(ObjectConverter))]
        public dynamic Value { get; set; }

        [JsonPropertyName("valueType")]
    
        public string ValueType { get; set; }


        [JsonPropertyName("description")]
        public string Description { get; set; }


        [JsonPropertyName("ackMode")]
        public string AckMode { get; set; }


        [JsonPropertyName("ackTime")]
        public DateTime? AckTime { get; set; }

        [JsonPropertyName("ackNotes")]
        public string AckNotes { get; set; }


        [JsonPropertyName("recoveryTime")]
        public DateTime? RecoveryTime { get; set; }

        [JsonPropertyName("operator")]
        public string Operator { get; set; }

        [JsonPropertyName("isShelved")]
        public bool IsShelved { get; set; }


        [JsonPropertyName("requireNotes")]
        public bool RequireNotes { get; set; }
    }
}
