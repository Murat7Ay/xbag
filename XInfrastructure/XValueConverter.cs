using System.Text.Json;
using System.Text.Json.Serialization;

namespace XInfrastructure;

public class XValueConverter : JsonConverter<XValue>
{
    public override XValue Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            if (doc.RootElement.ValueKind == JsonValueKind.Null)
            {
                return XValue.Create(XType.None, null);
            }
            else if (doc.RootElement.TryGetProperty("XType", out var typeProperty) &&
                     doc.RootElement.TryGetProperty("Value", out var valueProperty))
            {
                XType xType = (XType)Enum.Parse(typeof(XType), typeProperty.GetString() ?? string.Empty);
                object? value = JsonSerializer.Deserialize(valueProperty.GetRawText(), xType.ToType(), options);
                return XValue.Create(xType, value);
            }

            throw new JsonException("Invalid XValue format.");
        }
    }

    public override void Write(Utf8JsonWriter writer, XValue value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("XType", value.XType.ToString());
        writer.WritePropertyName("Value");
        JsonSerializer.Serialize(writer, value.Value, value.XType.ToType(), options);
        writer.WriteEndObject();
    }
}