﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace XInfrastructure;

public class XTableConverter : JsonConverter<XTable>
{
    public override XTable Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            if (doc.RootElement.ValueKind == JsonValueKind.Array)
            {
                var xTable = new XTable();
                int index = 0;
                foreach (JsonElement rowElement in doc.RootElement.EnumerateArray())
                {
                    if (rowElement.ValueKind == JsonValueKind.Object)
                    {
                        var row = new XBag();
                        foreach (JsonProperty property in rowElement.EnumerateObject())
                        {
                            string key = property.Name;
                            XValue value = JsonSerializer.Deserialize<XValue>(property.Value.GetRawText(), options);
                            row.Put(key, value);
                        }

                        xTable.BagToRow(index, row);
                        index++;
                    }
                }

                return xTable;
            }

            throw new JsonException("Invalid XTable format.");
        }
    }

    public override void Write(Utf8JsonWriter writer, XTable value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();
        foreach (var rowIndex in value.RowKeys)
        {
            XBag bag = value.RowToBag(rowIndex);
            JsonSerializer.Serialize(writer, bag, bag.GetType(), options);
        }
        writer.WriteEndArray();
    }
}