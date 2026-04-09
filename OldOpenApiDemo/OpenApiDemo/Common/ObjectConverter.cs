using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Threading.Tasks;

namespace OpenApiDemo.Common
{
    public class ObjectConverter : JsonConverter<object>
    {
        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var doc = JsonDocument.ParseValue(ref reader);
            if (doc != null)
            {
                switch (doc.RootElement.ValueKind)
                {
                    case JsonValueKind.Number:
                        return doc.RootElement.GetDouble();
                    case JsonValueKind.String:
                        return doc.RootElement.GetString();
                    case JsonValueKind.True:
                    case JsonValueKind.False:
                        return doc.RootElement.GetBoolean();
                    case JsonValueKind.Object:
                        JsonObject objects = new JsonObject();
                        foreach (var obj in doc.RootElement.EnumerateObject())
                        {
                            objects[obj.Name] = JsonSerializer.Deserialize<object>(obj.Value.GetRawText(), options);
                        }
                        return objects;
                    case JsonValueKind.Array:
                        JsonArray jsonArray = new JsonArray();
                        foreach (var item in doc.RootElement.EnumerateArray())
                        {
                            jsonArray.Add(JsonSerializer.Deserialize<object>(item.GetRawText(), options));
                        }
                        return jsonArray;
                    case JsonValueKind.Null:
                    case JsonValueKind.Undefined:
                        return null;
                }
            }
            return null;
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            if (value == null)
            {
                return;
            }

            if (value is JsonElement jsonElement)
            {
                JsonDocument jsonDocument = JsonDocument.Parse(jsonElement.GetRawText());
                jsonDocument.WriteTo(writer);
            }
            else
            {
                JsonSerializer.Serialize(writer, value, value.GetType(), options);
                //jsonDocument = JsonDocument.Parse(JsonSerializer.Serialize(value, options));
                //jsonDocument.WriteTo(writer);
            }
        }
    }
}
