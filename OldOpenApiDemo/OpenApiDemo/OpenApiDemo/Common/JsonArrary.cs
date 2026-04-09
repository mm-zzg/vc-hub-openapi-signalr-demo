using System.Text.Json.Serialization;

namespace OpenApiDemo.Common
{
    public class JsonArray : List<object>
    {

        public override string ToString()
        {
            var options = new System.Text.Json.JsonSerializerOptions();
            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new ObjectConverter());

            return System.Text.Json.JsonSerializer.Serialize(this, options);
           
        }
    }
}
