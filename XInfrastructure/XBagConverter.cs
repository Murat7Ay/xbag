using System.Text.Json;
using System.Text.Json.Serialization;

namespace XInfrastructure;

public class XBagConverter : JsonConverter<XBag>
{
    public override XBag Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            if (doc.RootElement.ValueKind == JsonValueKind.Object)
            {
                var xBag = new XBag();
                foreach (JsonProperty property in doc.RootElement.EnumerateObject())
                {
                    string key = property.Name;
                    XValue value = JsonSerializer.Deserialize<XValue>(property.Value.GetRawText(), options);
                    xBag.Put(key, value);
                }

                return xBag;
            }

            throw new JsonException("Invalid XBag format.");
        }
    }

    public override void Write(Utf8JsonWriter writer, XBag value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach (var pair in value.GetReadOnlyDictionary())
        {
            writer.WritePropertyName(pair.Key);
            Type valueType = pair.Value.GetType();
            JsonSerializer.Serialize(writer, pair.Value, valueType, options);
        }

        writer.WriteEndObject();
    }
}