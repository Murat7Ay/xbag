using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using XInfrastructure;
// ReSharper disable ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
// ReSharper disable RedundantCast
#pragma warning disable CS8600
#pragma warning disable CS8602

namespace XBagTests;

[TestFixture]
[SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)")]
public class XBagSerializationTests
{
    [Test]
    public void SerializeAndDeserializeNestedListsInXBag()
    {
        // Arrange
        List<bool> innerList1 = new List<bool> { true, false, true };
        List<long> innerList2 = new List<long> { 1, 2, 4 };
        XValue innerListValue1 = XValue.Create(XType.BoolList, innerList1);
        XValue innerListValue2 = XValue.Create(XType.LongList, innerList2);

        XBag bag = new XBag();
        bag.Put("NestedList1", innerListValue1);
        bag.Put("NestedList2", innerListValue2);

        var options = new JsonSerializerOptions { Converters = { new XBagConverter(), new XValueConverter(), new JsonStringEnumConverter(), new XDateTimeConverter() } };

        // Act
        string bagJson = JsonSerializer.Serialize(bag, options);
        var deserializedBag = JsonSerializer.Deserialize<XBag>(bagJson, options);

        // Assert
        Assert.IsNotNull(deserializedBag);

        IXData retrievedXValue1 = deserializedBag.Get("NestedList1");
        Assert.IsNotNull(retrievedXValue1);
        Assert.That(retrievedXValue1.XType, Is.EqualTo(XType.BoolList));
        List<bool> retrievedList1 = (List<bool>)retrievedXValue1.Value;
        Assert.AreEqual(3, retrievedList1.Count);
        Assert.AreEqual(true, retrievedList1[0]);
        Assert.AreEqual(false, retrievedList1[1]);
        Assert.AreEqual(true, retrievedList1[2]);

        IXData retrievedXValue2 = deserializedBag.Get("NestedList2");
        Assert.IsNotNull(retrievedXValue2);
        Assert.That(retrievedXValue2.XType, Is.EqualTo(XType.LongList));
        List<long> retrievedList2 = (List<long>)retrievedXValue2.Value;
        Assert.AreEqual(3, retrievedList2.Count);
        Assert.AreEqual(1, retrievedList2[0]);
        Assert.AreEqual(2, retrievedList2[1]);
        Assert.AreEqual(4, retrievedList2[2]);
    }
    [Test]
    public void SerializeAndDeserializeXValueWithMinimumAndMaximumValues()
    {
        // Arrange
        var xValueMinLong = XValue.Create(XType.Long, long.MinValue);
        var xValueMaxLong = XValue.Create(XType.Long, long.MaxValue);
        var xValueMinInt = XValue.Create(XType.Int, int.MinValue);
        var xValueMaxInt = XValue.Create(XType.Int, int.MaxValue);
        var xValueMinDecimal = XValue.Create(XType.Decimal, decimal.MinValue);
        var xValueMaxDecimal = XValue.Create(XType.Decimal, decimal.MaxValue);
        var xValueMinDouble = XValue.Create(XType.Double, double.MinValue);
        var xValueMaxDouble = XValue.Create(XType.Double, double.MaxValue);

        var options = new JsonSerializerOptions { Converters = { new XValueConverter(), new JsonStringEnumConverter(), new XDateTimeConverter() } };

        // Act
        string minLongJson = JsonSerializer.Serialize(xValueMinLong, options);
        string maxLongJson = JsonSerializer.Serialize(xValueMaxLong, options);
        string minIntJson = JsonSerializer.Serialize(xValueMinInt, options);
        string maxIntJson = JsonSerializer.Serialize(xValueMaxInt, options);
        string minDecimalJson = JsonSerializer.Serialize(xValueMinDecimal, options);
        string maxDecimalJson = JsonSerializer.Serialize(xValueMaxDecimal, options);
        string minDoubleJson = JsonSerializer.Serialize(xValueMinDouble, options);
        string maxDoubleJson = JsonSerializer.Serialize(xValueMaxDouble, options);

        var deserializedMinLong = JsonSerializer.Deserialize<XValue>(minLongJson, options);
        var deserializedMaxLong = JsonSerializer.Deserialize<XValue>(maxLongJson, options);
        var deserializedMinInt = JsonSerializer.Deserialize<XValue>(minIntJson, options);
        var deserializedMaxInt = JsonSerializer.Deserialize<XValue>(maxIntJson, options);
        var deserializedMinDecimal = JsonSerializer.Deserialize<XValue>(minDecimalJson, options);
        var deserializedMaxDecimal = JsonSerializer.Deserialize<XValue>(maxDecimalJson, options);
        var deserializedMinDouble = JsonSerializer.Deserialize<XValue>(minDoubleJson, options);
        var deserializedMaxDouble = JsonSerializer.Deserialize<XValue>(maxDoubleJson, options);

        // Assert
        Assert.That(deserializedMinLong.Value, Is.EqualTo(long.MinValue));
        Assert.That(deserializedMaxLong.Value, Is.EqualTo(long.MaxValue));
        Assert.That(deserializedMinInt.Value, Is.EqualTo(int.MinValue));
        Assert.That(deserializedMaxInt.Value, Is.EqualTo(int.MaxValue));
        Assert.That(deserializedMinDecimal.Value, Is.EqualTo(decimal.MinValue));
        Assert.That(deserializedMaxDecimal.Value, Is.EqualTo(decimal.MaxValue));
        Assert.That(deserializedMinDouble.Value, Is.EqualTo(double.MinValue));
        Assert.That(deserializedMaxDouble.Value, Is.EqualTo(double.MaxValue));
    }
    [Test]
    public void SerializeAndDeserializeComplexXBag()
    {
        // Arrange
        var innerTable = new XTable();
        innerTable.Put(0, "InnerTableValue", XValue.Create(XType.String, "NestedTableValue"));

        var innerBag = new XBag();
        innerBag.Put("InnerBagValue", XValue.Create(XType.Bool, true));
        innerBag.Put("InnerTable", XValue.Create(XType.Table, innerTable));

        var complexBag = new XBag();
        complexBag.Put("StringValue", XValue.Create(XType.String, "Hello, World!"));
        complexBag.Put("LongValue", XValue.Create(XType.Long, long.MaxValue));
        complexBag.Put("BoolValue", XValue.Create(XType.Bool, false));
        complexBag.Put("DecimalValue", XValue.Create(XType.Decimal, 123.45m));
        complexBag.Put("DoubleValue", XValue.Create(XType.Double, 3.14));
        complexBag.Put("DateValue", XValue.Create(XType.Date, new DateTime(2023, 9, 17)));
        complexBag.Put("InnerBag", XValue.Create(XType.Bag, innerBag));

        var options = new JsonSerializerOptions { Converters = { new XBagConverter(), new XValueConverter(), new XTableConverter(), new JsonStringEnumConverter(), new XDateTimeConverter() } };

        // Act
        string bagJson = JsonSerializer.Serialize(complexBag, options);
        var deserializedBag = JsonSerializer.Deserialize<XBag>(bagJson, options);

        // Assert
        Assert.IsNotNull(deserializedBag);

        // Compare the top-level values
        Assert.AreEqual("Hello, World!", deserializedBag.Get("StringValue")?.Value);
        Assert.AreEqual(long.MaxValue, deserializedBag.Get("LongValue")?.Value);
        Assert.AreEqual(false, deserializedBag.Get("BoolValue")?.Value);
        Assert.AreEqual(123.45m, deserializedBag.Get("DecimalValue")?.Value);
        Assert.AreEqual(3.14, deserializedBag.Get("DoubleValue")?.Value);
        Assert.AreEqual(new DateTime(2023, 9, 17), deserializedBag.Get("DateValue")?.Value);

        // Compare the inner bag
        var innerBagValue = deserializedBag.Get("InnerBag") as XValue;
        Assert.IsNotNull(innerBagValue);
        var innerBagF = innerBagValue.Value as XBag;
        Assert.IsNotNull(innerBagF);
        Assert.AreEqual(true, innerBagF.Get("InnerBagValue")?.Value);

        // Compare the inner table inside the inner bag
        var innerTableValue = innerBag.Get("InnerTable") as XValue;
        Assert.IsNotNull(innerTableValue);
        var innerTableF = innerTableValue.Value as XTable;
        Assert.IsNotNull(innerTableF);
        Assert.AreEqual("NestedTableValue", innerTableF.Get(0, "InnerTableValue")?.Value);
    }
    [Test]
    public void SerializeDeserializeSerializeXBag()
    {
        // Arrange
        var innerBag = new XBag();
        innerBag.Put("String", XValue.Create(XType.String, "Hello, World!"));
        innerBag.Put("Long", XValue.Create(XType.Long, long.MaxValue));
        innerBag.Put("Bool", XValue.Create(XType.Bool, true));
        innerBag.Put("Decimal", XValue.Create(XType.Decimal, 123.45m));
        innerBag.Put("Double", XValue.Create(XType.Double, 3.14));
        innerBag.Put("Date", XValue.Create(XType.Date, new DateTime(2023, 9, 17)));
        innerBag.Put("InnerTable", CreateInnerTable());

        var xBag = new XBag();
        xBag.Put("String", XValue.Create(XType.String, "Hello, World!"));
        xBag.Put("Long", XValue.Create(XType.Long, long.MinValue));
        xBag.Put("Bool", XValue.Create(XType.Bool, false));
        xBag.Put("Decimal", XValue.Create(XType.Decimal, -123.45m));
        xBag.Put("Double", XValue.Create(XType.Double, -3.14));
        xBag.Put("Date", XValue.Create(XType.Date, new DateTime(2023, 9, 16)));
        xBag.Put("InnerBag", XValue.Create(XType.Bag, innerBag));

        var options = new JsonSerializerOptions { Converters = { new XBagConverter(), new XValueConverter(), new XTableConverter(), new JsonStringEnumConverter(), new XDateTimeConverter() } };

        // Act
        string initialJson = JsonSerializer.Serialize(xBag, options);
        var deserializedBag = JsonSerializer.Deserialize<XBag>(initialJson, options);
        string finalJson = JsonSerializer.Serialize(deserializedBag, options);

        // Assert
        Assert.AreEqual(initialJson, finalJson);
    }

    private XValue CreateInnerTable()
    {
        var innerTable = new XTable();
        innerTable.Put(0, "String", XValue.Create(XType.String, "Nested String"));
        innerTable.Put(0, "Long", XValue.Create(XType.Long, long.MaxValue));
        innerTable.Put(0, "Bool", XValue.Create(XType.Bool, true));
        innerTable.Put(0, "Decimal", XValue.Create(XType.Decimal, 123.45m));
        innerTable.Put(0, "Double", XValue.Create(XType.Double, 3.14));
        innerTable.Put(0, "Date", XValue.Create(XType.Date, new DateTime(2023, 9, 17)));

        var innerTable2 = new XTable();
        innerTable2.Put(0, "String", XValue.Create(XType.String, "Nested String 2"));
        innerTable2.Put(0, "Long", XValue.Create(XType.Long, long.MinValue));
        innerTable2.Put(0, "Bool", XValue.Create(XType.Bool, false));
        innerTable2.Put(0, "Decimal", XValue.Create(XType.Decimal, -123.45m));
        innerTable2.Put(0, "Double", XValue.Create(XType.Double, -3.14));
        innerTable2.Put(0, "Date", XValue.Create(XType.Date, new DateTime(2023, 9, 16)));

        var xTable = new XTable();
        xTable.Put(0, "InnerTable1", XValue.Create(XType.Table, innerTable));
        xTable.Put(1, "InnerTable2", XValue.Create(XType.Table, innerTable2));

        return XValue.Create(XType.Table, xTable);
    }
    [Test]
    public void SerializeAndDeserializeNestedListsInXTable()
    {
        // Arrange
        List<long> innerList1 = new List<long> { 1, 2, 3 };
        List<decimal> innerList2 = new List<decimal> { 1.1M, 2.5M, 4.4M };
        XValue innerListValue1 = XValue.Create(XType.LongList, innerList1);
        XValue innerListValue2 = XValue.Create(XType.DecimalList, innerList2);

        XTable table = new XTable();
        table.Put(0, "NestedList1", innerListValue1);
        table.Put(1, "NestedList2", innerListValue2);

        var options = new JsonSerializerOptions { Converters = { new XTableConverter(), new XValueConverter(), new XBagConverter(), new JsonStringEnumConverter(), new XDateTimeConverter() } };

        // Act
        string tableJson = JsonSerializer.Serialize(table, options);
        var deserializedTable = JsonSerializer.Deserialize<XTable>(tableJson, options);

        // Assert
        Assert.IsNotNull(deserializedTable);

        IXData retrievedXValue1 = deserializedTable.Get(0, "NestedList1");
        Assert.IsNotNull(retrievedXValue1);
        Assert.That(retrievedXValue1.XType, Is.EqualTo(XType.LongList));
        List<long> retrievedList1 = (List<long>)retrievedXValue1.Value;
        Assert.AreEqual(3, retrievedList1.Count);
        Assert.AreEqual(1, retrievedList1[0]);
        Assert.AreEqual(2, retrievedList1[1]);
        Assert.AreEqual(3, retrievedList1[2]);

        IXData retrievedXValue2 = deserializedTable.Get(1, "NestedList2");
        Assert.IsNotNull(retrievedXValue2);
        Assert.That(retrievedXValue2.XType, Is.EqualTo(XType.DecimalList));
        List<decimal> retrievedList2 = (List<decimal>)retrievedXValue2.Value;
        Assert.AreEqual(3, retrievedList2.Count);
        Assert.AreEqual(1.1M, retrievedList2[0]);
        Assert.AreEqual(2.5M, retrievedList2[1]);
        Assert.AreEqual(4.4M, retrievedList2[2]);
    }

    [Test]
    public void SerializeAndDeserializeEmptyNestedListInXBag()
    {
        // Arrange
        List<string> innerList = new List<string>();
        XValue innerListValue = XValue.Create(XType.StringList, innerList);

        XBag bag = new XBag();
        bag.Put("NestedList", innerListValue);

        var options = new JsonSerializerOptions { Converters = { new XBagConverter(), new XValueConverter(), new JsonStringEnumConverter() , new XDateTimeConverter()} };

        // Act
        string bagJson = JsonSerializer.Serialize(bag, options);
        var deserializedBag = JsonSerializer.Deserialize<XBag>(bagJson, options);

        // Assert
        Assert.IsNotNull(deserializedBag);

        IXData retrievedXValue = deserializedBag.Get("NestedList");
        Assert.IsNotNull(retrievedXValue);
        Assert.That(retrievedXValue.XType, Is.EqualTo(XType.StringList));
        List<string> retrievedList = (List<string>)retrievedXValue.Value;
        Assert.AreEqual(0, retrievedList.Count);
    }

    [Test]
    public void SerializeAndDeserializeEmptyNestedListInXTable()
    {
        // Arrange
        List<string> innerList = new List<string>();
        XValue innerListValue = XValue.Create(XType.StringList, innerList);

        XTable table = new XTable();
        table.Put(0, "NestedList", innerListValue);

        var options = new JsonSerializerOptions { Converters = { new XTableConverter(), new XValueConverter(), new XBagConverter(), new JsonStringEnumConverter() , new XDateTimeConverter()} };

        // Act
        string tableJson = JsonSerializer.Serialize(table, options);
        var deserializedTable = JsonSerializer.Deserialize<XTable>(tableJson, options);

        // Assert
        Assert.IsNotNull(deserializedTable);

        IXData retrievedXValue = deserializedTable.Get(0, "NestedList");
        Assert.IsNotNull(retrievedXValue);
        Assert.That(retrievedXValue.XType, Is.EqualTo(XType.StringList));
        List<string> retrievedList = (List<string>)retrievedXValue.Value;
        Assert.AreEqual(0, retrievedList.Count);
    }
    [Test]
    public void SerializeAndDeserializeXBag()
    {
        // Arrange
        var xBag = new XBag();
        xBag.Put("Name", XValue.Create(XType.String, "John"));
        xBag.Put("Age", XValue.Create(XType.Int, 30));
        var options = new JsonSerializerOptions { Converters = { new XBagConverter(), new XValueConverter(), new JsonStringEnumConverter() , new XDateTimeConverter()} };

        // Act
        string bagJson = JsonSerializer.Serialize(xBag, options);
        var deserializedBag = JsonSerializer.Deserialize<XBag>(bagJson, options);

        // Assert
        Assert.IsNotNull(deserializedBag);
        Assert.AreEqual(2, deserializedBag.GetReadOnlyDictionary().Count);
        Assert.AreEqual("John", deserializedBag.Get("Name")?.Value);
        Assert.AreEqual(30, deserializedBag.Get("Age")?.Value);
    }

    [Test]
    public void SerializeAndDeserializeXTable()
    {
        // Arrange
        var xTable = new XTable();
        xTable.Put(0, "Name", XValue.Create(XType.String, "Alice"));
        xTable.Put(0, "Age", XValue.Create(XType.Int, 25));
        xTable.Put(1, "Name", XValue.Create(XType.String, "Bob"));
        xTable.Put(1, "Age", XValue.Create(XType.Int, 35));
        var options = new JsonSerializerOptions { Converters = { new XTableConverter(), new XValueConverter(), new XBagConverter(), new JsonStringEnumConverter(), new XDateTimeConverter() } };

        // Act
        string tableJson = JsonSerializer.Serialize(xTable, options);
        var deserializedTable = JsonSerializer.Deserialize<XTable>(tableJson, options);

        // Assert
        Assert.IsNotNull(deserializedTable);
        Assert.That(deserializedTable.RowKeys.Count, Is.EqualTo(2));
        Assert.That(deserializedTable.Get(0, "Name")?.Value, Is.EqualTo("Alice"));
        Assert.That(deserializedTable.Get(0, "Age")?.Value, Is.EqualTo(25));
        Assert.That(deserializedTable.Get(1, "Name")?.Value, Is.EqualTo("Bob"));
        Assert.That(deserializedTable.Get(1, "Age")?.Value, Is.EqualTo(35));
    }
}