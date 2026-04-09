using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiDemo.Models
{
    public record LicenseStatusRecord
    {

        [JsonPropertyName("license")]
        public string License { get; set; }

        [JsonPropertyName("remainingTrailTime")]
        public int RemainingTrailTime { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("signature")]
        public byte[] Signature { get; set; }


        public override string ToString()
        {
            return $"{License}|{RemainingTrailTime}|{Timestamp}";
        }
    }
}
