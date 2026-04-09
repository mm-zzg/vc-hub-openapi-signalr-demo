using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OpenApiDemo.Models
{
    public class AssetTagModel
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("treeName")]
        public string TreeName { get; set; }


     
    }
}
