
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiDemo.Models
{
   
    public record RedundancyStatusRecord
    {

        [JsonPropertyName("state")]
       
        public required string State { get; set; }

        [JsonPropertyName("redundantState")]
      
        public required string RedundantState { get; set; }

        [JsonPropertyName("redundantUrls")]
        public required IEnumerable<string> RedundantUrls { get; set; } = new List<string>();

        [JsonPropertyName("error")]
        public string Error { get; set; }

    }
}
