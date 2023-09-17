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
}