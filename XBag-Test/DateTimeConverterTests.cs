using System.Text.Json;
using XInfrastructure;

namespace XBagTests;

[TestFixture]
public class DateTimeConverterTests
{
    [Test]
    public void SerializeDeserializeXBagWithDateTimeInCustomFormat()
    {
        // Arrange
        DateTime originalDateTime = new DateTime(2023, 9, 17, 12, 34, 56, 789);
        XBag xBag = new XBag();
        xBag.Put("CustomDateTime", XValue.Create(XType.Date, originalDateTime));
        var options = new JsonSerializerOptions { Converters = { new XBagConverter(), new XValueConverter(), new XDateTimeConverter() } };

        // Act
        string json = JsonSerializer.Serialize(xBag, options);
        XBag deserializedBag = JsonSerializer.Deserialize<XBag>(json, options);
        DateTime deserializedDateTime = (DateTime)deserializedBag.Get("CustomDateTime")?.Value;

        // Assert
        Assert.AreEqual(originalDateTime, deserializedDateTime);
        Assert.AreEqual(true, json.Contains("20230917123456789")); // Ensure the custom format is used in serialization
    }
}