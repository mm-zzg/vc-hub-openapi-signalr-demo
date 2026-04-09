using System.Text.Json.Serialization;

namespace OpenApiDemo.Models
{
    public class AssetTreeModel 
    {

        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("historyDatabase")]
        public string HistoryDatabase { get; set; } = "Default";

    }
}
