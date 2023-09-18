using System.Text.Json;
using XInfrastructure;

namespace XBagTests;

[TestFixture]
public class XValueTests
{
    [Test]
    public void CreateXValueWithMinimumAndMaximumValues()
    {
        // Arrange & Act
        var xValueMinLong = XValue.Create(XType.Long, long.MinValue);
        var xValueMaxLong = XValue.Create(XType.Long, long.MaxValue);
        var xValueMinInt = XValue.Create(XType.Int, int.MinValue);
        var xValueMaxInt = XValue.Create(XType.Int, int.MaxValue);
        var xValueMinDecimal = XValue.Create(XType.Decimal, decimal.MinValue);
        var xValueMaxDecimal = XValue.Create(XType.Decimal, decimal.MaxValue);
        var xValueMinDouble = XValue.Create(XType.Double, double.MinValue);
        var xValueMaxDouble = XValue.Create(XType.Double, double.MaxValue);

        // Assert
        Assert.That(xValueMinLong.Value, Is.EqualTo(long.MinValue));
        Assert.That(xValueMaxLong.Value, Is.EqualTo(long.MaxValue));
        Assert.That(xValueMinInt.Value, Is.EqualTo(int.MinValue));
        Assert.That(xValueMaxInt.Value, Is.EqualTo(int.MaxValue));
        Assert.That(xValueMinDecimal.Value, Is.EqualTo(decimal.MinValue));
        Assert.That(xValueMaxDecimal.Value, Is.EqualTo(decimal.MaxValue));
        Assert.That(xValueMinDouble.Value, Is.EqualTo(double.MinValue));
        Assert.That(xValueMaxDouble.Value, Is.EqualTo(double.MaxValue));
    }

    [Test]
    public void PutAndGetListValue()
    {
        // Arrange
        XBag bag = new XBag();
        var listValue = new List<long> { 1, 2, 3 };
        var value = XValue.Create(XType.LongList, listValue);

        // Act
        bag.Put("list", value);
        var retrievedValue = bag.Get("list");

        // Assert
        Assert.That(retrievedValue.Value, Is.EqualTo(listValue));
    }

    [Test]
    public void CreateXValue()
    {
        // Arrange
        var xType = XType.Long;
        var value = 42L;

        // Act
        var xValue = XValue.Create(xType, value);

        // Assert
        Assert.That(xValue.XType, Is.EqualTo(xType));
        Assert.That(xValue.Value, Is.EqualTo(value));
    }

    [Test]
    public void CreateXValueWithUnsupportedType_ShouldThrowException()
    {
        // Arrange & Act & Assert
        Assert.Throws<ArgumentException>(() => XValue.Create(XType.String, new object()));
    }

    [Test]
    public void CompareXValuesWithDifferentTypes()
    {
        var xValueLong = XValue.Create(XType.Long, 42L);
        var xValueString = XValue.Create(XType.String, "42");

        Assert.That(xValueLong.Equals(xValueString), Is.False);
        Assert.That(xValueLong.GetHashCode(), Is.Not.EqualTo(xValueString.GetHashCode()));
    }

    [Test]
    public void ImplicitConversionToXValue()
    {
        // Test implicit conversion from supported types to XValue
        long longValue = 42L;
        XValue xValueLong = longValue;
        Assert.That(xValueLong.XType, Is.EqualTo(XType.Long));
        Assert.That(xValueLong.Value, Is.EqualTo(longValue));

        string stringValue = "Hello";
        XValue xValueString = stringValue;
        Assert.That(xValueString.XType, Is.EqualTo(XType.String));
        Assert.That(xValueString.Value, Is.EqualTo(stringValue));

        bool boolValue = true;
        XValue xValueBool = boolValue;
        Assert.That(xValueBool.XType, Is.EqualTo(XType.Bool));
        Assert.That(xValueBool.Value, Is.EqualTo(boolValue));

        decimal decimalValue = 42.42m;
        XValue xValueDecimal = decimalValue;
        Assert.That(xValueDecimal.XType, Is.EqualTo(XType.Decimal));
        Assert.That(xValueDecimal.Value, Is.EqualTo(decimalValue));

        double doubleValue = 42.42;
        XValue xValueDouble = doubleValue;
        Assert.That(xValueDouble.XType, Is.EqualTo(XType.Double));
        Assert.That(xValueDouble.Value, Is.EqualTo(doubleValue));

        DateTime dateTimeValue = DateTime.Now;
        XValue xValueDateTime = dateTimeValue;
        Assert.That(xValueDateTime.XType, Is.EqualTo(XType.Date));
        Assert.That(xValueDateTime.Value, Is.EqualTo(dateTimeValue));

        XBag xBagValue = new XBag();
        XValue xValueXBag = xBagValue;
        Assert.That(xValueXBag.XType, Is.EqualTo(XType.Bag));
        Assert.That(xValueXBag.Value, Is.EqualTo(xBagValue));

        List<long> longListValue = new List<long> { 1, 2, 3 };
        XValue xValueLongList = longListValue;
        Assert.That(xValueLongList.XType, Is.EqualTo(XType.LongList));
        Assert.That(xValueLongList.Value, Is.EqualTo(longListValue));

        List<string> stringListValue = new List<string> { "a", "b", "c" };
        XValue xValueStringList = stringListValue;
        Assert.That(xValueStringList.XType, Is.EqualTo(XType.StringList));
        Assert.That(xValueStringList.Value, Is.EqualTo(stringListValue));

        List<bool> boolListValue = new List<bool> { true, false, true };
        XValue xValueBoolList = boolListValue;
        Assert.That(xValueBoolList.XType, Is.EqualTo(XType.BoolList));
        Assert.That(xValueBoolList.Value, Is.EqualTo(boolListValue));

        List<decimal> decimalListValue = new List<decimal> { 1.1m, 2.2m, 3.3m };
        XValue xValueDecimalList = decimalListValue;
        Assert.That(xValueDecimalList.XType, Is.EqualTo(XType.DecimalList));
        Assert.That(xValueDecimalList.Value, Is.EqualTo(decimalListValue));

        List<double> doubleListValue = new List<double> { 1.1, 2.2, 3.3 };
        XValue xValueDoubleList = doubleListValue;
        Assert.That(xValueDoubleList.XType, Is.EqualTo(XType.DoubleList));
        Assert.That(xValueDoubleList.Value, Is.EqualTo(doubleListValue));
    }


    [Test]
    public void CompareXValueInstancesForEquality()
    {
        // Arrange
        var xType = XType.Long;
        var value1 = 42L;
        var value2 = 42L;

        // Act
        var xValue1 = XValue.Create(xType, value1);
        var xValue2 = XValue.Create(xType, value2);

        // Assert
        Assert.That(xValue1.Equals(xValue2), Is.True);
    }

    [Test]
    public void CompareXValueInstancesForInequality()
    {
        // Arrange
        var xType = XType.Long;
        var value1 = 42L;
        var value2 = 43L;

        // Act
        var xValue1 = XValue.Create(xType, value1);
        var xValue2 = XValue.Create(xType, value2);

        // Assert
        Assert.That(xValue1.Equals(xValue2), Is.False);
    }

    [Test]
    public void CreateXValueWithBoundaryValues()
    {
        // Arrange & Act
        var xValue1 = XValue.Create(XType.Long, long.MaxValue);
        var xValue2 = XValue.Create(XType.Long, long.MinValue);

        // Assert
        Assert.That(xValue1.Value, Is.EqualTo(long.MaxValue));
        Assert.That(xValue2.Value, Is.EqualTo(long.MinValue));
    }

    [Test]
    public void JsonSerializationDeserialization()
    {
        var originalValue = XValue.Create(XType.Long, 42L);
        var options = new JsonSerializerOptions { Converters = { new XValueConverter() } };
        var json = JsonSerializer.Serialize(originalValue, options);
        var deserializedValue = JsonSerializer.Deserialize<XValue>(json, options);

        Assert.That(deserializedValue, Is.EqualTo(originalValue));
    }
}