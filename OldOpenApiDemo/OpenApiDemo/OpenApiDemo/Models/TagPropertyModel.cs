using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SCADA.Modules.OpenApi.Models
{

    public class TagPropertyModel
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("property")]
        public string Property { get; set; }
        [JsonPropertyName("value")]
        public dynamic Value { get; set; }
    }
}
