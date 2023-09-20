using System.Diagnostics.CodeAnalysis;
using XInfrastructure;

namespace XBagTests;

[TestFixture]
[SuppressMessage("Assertion", "NUnit2005:Consider using Assert.That(actual, Is.EqualTo(expected)) instead of Assert.AreEqual(expected, actual)")]
public class XValueExtensionsTests
{
    [Test]
    public void To_String_NoneValue_ReturnsEmptyString()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        string result = xValue.To_String();

        // Assert
        Assert.AreEqual("", result);
    }

    [Test]
    public void To_String_IntValue_ReturnsStringRepresentation()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Int, 42);

        // Act
        string result = xValue.To_String();

        // Assert
        Assert.AreEqual("42", result);
    }

    [Test]
    public void To_String_BoolValue_ReturnsStringRepresentation()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Bool, true);

        // Act
        string result = xValue.To_String();

        // Assert
        Assert.AreEqual("True", result);
    }

    [Test]
    public void To_String_DecimalValue_ReturnsFormattedString()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Decimal, 123.456m);

        // Act
        string result = xValue.To_String();

        // Assert
        Assert.AreEqual("123.456", result);
    }

    [Test]
    public void To_String_DoubleValue_ReturnsFormattedString()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Double, 123.4567890123);

        // Act
        string result = xValue.To_String();

        // Assert
        Assert.AreEqual("123.4567890123", result);
    }

    [Test]
    public void To_String_UnsupportedType_ReturnsEmptyString()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Table, null); // Using an unsupported XType

        // Act
        string result = xValue.To_String();

        // Assert
        Assert.AreEqual("", result);
    }
    
    [Test]
    public void To_Long_NoneValue_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        long result = xValue.To_Long();

        // Assert
        Assert.AreEqual(0L, result);
    }

    [Test]
    public void To_Long_IntValue_ReturnsLongValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Int, 42);

        // Act
        long result = xValue.To_Long();

        // Assert
        Assert.AreEqual(42L, result);
    }

    [Test]
    public void To_Long_BoolTrueValue_ReturnsOne()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Bool, true);

        // Act
        long result = xValue.To_Long();

        // Assert
        Assert.AreEqual(1L, result);
    }

    [Test]
    public void To_Long_BoolFalseValue_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Bool, false);

        // Act
        long result = xValue.To_Long();

        // Assert
        Assert.AreEqual(0L, result);
    }

    [Test]
    public void To_Long_DecimalValue_ReturnsLongValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Decimal, 123.45m);

        // Act
        long result = xValue.To_Long();

        // Assert
        Assert.AreEqual(123L, result);
    }

    [Test]
    public void To_Long_DoubleValue_ReturnsLongValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Double, 123.45);

        // Act
        long result = xValue.To_Long();

        // Assert
        Assert.AreEqual(123L, result);
    }

    [Test]
    public void To_Long_UnsupportedType_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.String, "Hello"); // Using an unsupported XType

        // Act
        long result = xValue.To_Long();

        // Assert
        Assert.AreEqual(0L, result);
    }
    
        [Test]
    public void To_Double_NoneValue_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        double result = xValue.To_Double();

        // Assert
        Assert.AreEqual(0.0, result);
    }

    [Test]
    public void To_Double_IntValue_ReturnsDoubleValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Int, 42);

        // Act
        double result = xValue.To_Double();

        // Assert
        Assert.AreEqual(42.0, result);
    }

    [Test]
    public void To_Double_BoolTrueValue_ReturnsOne()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Bool, true);

        // Act
        double result = xValue.To_Double();

        // Assert
        Assert.AreEqual(1.0, result);
    }

    [Test]
    public void To_Double_BoolFalseValue_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Bool, false);

        // Act
        double result = xValue.To_Double();

        // Assert
        Assert.AreEqual(0.0, result);
    }

    [Test]
    public void To_Double_DecimalValue_ReturnsDoubleValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Decimal, 123.45m);

        // Act
        double result = xValue.To_Double();

        // Assert
        Assert.AreEqual(123.45, result);
    }

    [Test]
    public void To_Double_DoubleValue_ReturnsDoubleValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Double, 123.45);

        // Act
        double result = xValue.To_Double();

        // Assert
        Assert.AreEqual(123.45, result);
    }

    [Test]
    public void To_Double_UnsupportedType_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.String, "Hello"); // Using an unsupported XType

        // Act
        double result = xValue.To_Double();

        // Assert
        Assert.AreEqual(0.0, result);
    }

    [Test]
    public void To_Decimal_NoneValue_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        decimal result = xValue.To_Decimal();

        // Assert
        Assert.AreEqual(0m, result);
    }

    [Test]
    public void To_Decimal_IntValue_ReturnsDecimalValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Int, 42);

        // Act
        decimal result = xValue.To_Decimal();

        // Assert
        Assert.AreEqual(42m, result);
    }

    [Test]
    public void To_Decimal_BoolTrueValue_ReturnsOne()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Bool, true);

        // Act
        decimal result = xValue.To_Decimal();

        // Assert
        Assert.AreEqual(1m, result);
    }

    [Test]
    public void To_Decimal_BoolFalseValue_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Bool, false);

        // Act
        decimal result = xValue.To_Decimal();

        // Assert
        Assert.AreEqual(0m, result);
    }

    [Test]
    public void To_Decimal_DecimalValue_ReturnsDecimalValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Decimal, 123.45m);

        // Act
        decimal result = xValue.To_Decimal();

        // Assert
        Assert.AreEqual(123.45m, result);
    }

    [Test]
    public void To_Decimal_DoubleValue_ReturnsDecimalValue()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Double, 123.45);

        // Act
        decimal result = xValue.To_Decimal();

        // Assert
        Assert.AreEqual(123.45m, result);
    }

    [Test]
    public void To_Decimal_UnsupportedType_ReturnsZero()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.String, "Hello"); // Using an unsupported XType

        // Act
        decimal result = xValue.To_Decimal();

        // Assert
        Assert.AreEqual(0m, result);
    }
    
    // Existing test cases...

    [Test]
    public void To_DateTime_DateValue_ReturnsDateTime()
    {
        // Arrange
        string expectedDateTime = DateTime.Now.ToString(XConstant.DateTimeFormat);
        XValue xValue = XValue.Create(XType.String, expectedDateTime);

        // Act
        string result = xValue.To_DateTime().ToString(XConstant.DateTimeFormat);

        // Assert
        Assert.AreEqual(expectedDateTime, result);
    }

    [Test]
    public void To_DateTime_NoneValue_ReturnsDefaultDateTime()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        DateTime result = xValue.To_DateTime();

        // Assert
        Assert.AreEqual(default(DateTime), result);
    }

    [Test]
    public void To_DateTime_NullValue_ReturnsDefaultDateTime()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Date, null);

        // Act
        DateTime result = xValue.To_DateTime();

        // Assert
        Assert.AreEqual(default(DateTime), result);
    }

    [Test]
    public void To_DateTime_IntValue_ReturnsDefaultDateTime()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.Int, 42);

        // Act
        DateTime result = xValue.To_DateTime();

        // Assert
        Assert.AreEqual(default(DateTime), result);
    }

    [Test]
    public void To_DateTime_UnsupportedType_ReturnsDefaultDateTime()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.String, "Hello"); // Using an unsupported XType

        // Act
        DateTime result = xValue.To_DateTime();

        // Assert
        Assert.AreEqual(default(DateTime), result);
    }
     [Test]
    public void To_LongList_NoneValue_ReturnsEmptyList()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        List<long> result = xValue.To_LongList();

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public void To_LongList_LongListValue_ReturnsList()
    {
        // Arrange
        List<long> expectedList = new List<long> { 1, 2, 3 };
        XValue xValue = XValue.Create(XType.LongList, expectedList);

        // Act
        List<long> result = xValue.To_LongList();

        // Assert
        Assert.AreEqual(expectedList, result);
    }

    [Test]
    public void To_DecimalList_NoneValue_ReturnsEmptyList()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        List<decimal> result = xValue.To_DecimalList();

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public void To_DecimalList_DecimalListValue_ReturnsList()
    {
        // Arrange
        List<decimal> expectedList = new List<decimal> { 1.1m, 2.2m, 3.3m };
        XValue xValue = XValue.Create(XType.DecimalList, expectedList);

        // Act
        List<decimal> result = xValue.To_DecimalList();

        // Assert
        Assert.AreEqual(expectedList, result);
    }

    [Test]
    public void To_DoubleList_NoneValue_ReturnsEmptyList()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        List<double> result = xValue.To_DoubleList();

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public void To_DoubleList_DoubleListValue_ReturnsList()
    {
        // Arrange
        List<double> expectedList = new List<double> { 1.1, 2.2, 3.3 };
        XValue xValue = XValue.Create(XType.DoubleList, expectedList);

        // Act
        List<double> result = xValue.To_DoubleList();

        // Assert
        Assert.AreEqual(expectedList, result);
    }

    [Test]
    public void To_BoolList_NoneValue_ReturnsEmptyList()
    {
        // Arrange
        XValue xValue = XValue.Create(XType.None, null);

        // Act
        List<bool> result = xValue.To_BoolList();

        // Assert
        Assert.IsEmpty(result);
    }

    [Test]
    public void To_BoolList_BoolListValue_ReturnsList()
    {
        // Arrange
        List<bool> expectedList = new List<bool> { true, false, true };
        XValue xValue = XValue.Create(XType.BoolList, expectedList);

        // Act
        List<bool> result = xValue.To_BoolList();

        // Assert
        Assert.AreEqual(expectedList, result);
    }
    
}