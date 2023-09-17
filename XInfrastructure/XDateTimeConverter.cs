using System.Text.Json;
using System.Text.Json.Serialization;

namespace XInfrastructure;

public class XDateTimeConverter : JsonConverter<DateTime>
{
    private const string DateTimeFormat = "yyyyMMdd HHmmss fff";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            if (DateTime.TryParseExact(reader.GetString(), DateTimeFormat, null, System.Globalization.DateTimeStyles.None, out DateTime parsedDateTime))
            {
                return parsedDateTime;
            }
        }

        throw new JsonException($"Unable to parse DateTime in the expected format ({DateTimeFormat}).");
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString(DateTimeFormat));
    }
}