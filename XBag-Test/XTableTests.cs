using System.Text.Json;
using XInfrastructure;

namespace XBagTests;

[TestFixture]
public class XTableTests
{
    [Test]
    public void Put_ValidValues_ShouldSetCellValue()
    {
        // Arrange
        XTable xTable = new XTable();
        xTable.Put(0, "Column1", XValue.Create(XType.Long, 42));
        xTable.Put(1, "Column2", XValue.Create(XType.String, "Value"));

        // Act & Assert
        Assert.That(xTable.Get(0, "Column1").Value, Is.EqualTo(42));
        Assert.That(xTable.Get(1, "Column2").Value, Is.EqualTo("Value"));
    }

    [Test]
    public void RowToBag_ShouldConvertRowToXBag()
    {
        // Arrange
        XTable xTable = new XTable();
        xTable.Put(0, "Column1", XValue.Create(XType.Long, 42));
        xTable.Put(0, "Column2", XValue.Create(XType.String, "Value"));

        // Act
        XBag xBag = xTable.RowToBag(0);

        // Assert
        Assert.IsNotNull(xBag);
        Assert.That(xBag.GetReadOnlyDictionary().Count, Is.EqualTo(2));
        Assert.That(xBag.Get("Column1").Value, Is.EqualTo(42));
        Assert.That(xBag.Get("Column2").Value, Is.EqualTo("Value"));
    }

    [Test]
    public void PutWithInvalidColumnName_ShouldThrowException()
    {
        // Arrange
        XTable xTable = new XTable();

        // Act & Assert
        Assert.Throws<JsonException>(() =>
            xTable.Put(0, "1-invalid-column-name", XValue.Create(XType.String, "Value")));
    }

    [Test]
    public void AddAndRetrieveRowsWithDifferentIndices()
    {
        // Arrange
        XTable xTable = new XTable();
        xTable.Put(0, "Column1", XValue.Create(XType.Long, 42));
        xTable.Put(1, "Column2", XValue.Create(XType.String, "Value"));

        // Act & Assert
        Assert.That(xTable.Get(0, "Column1").Value, Is.EqualTo(42));
        Assert.That(xTable.Get(1, "Column2").Value, Is.EqualTo("Value"));
    }

    [Test]
    public void AddAndRetrieveRowsWithInvalidRowIndices()
    {
        // Arrange
        XTable xTable = new XTable();

        // Act & Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => xTable.Put(-1, "Column1", XValue.Create(XType.Long, 42)));
    }

    [Test]
    public void AddAndRetrieveRowsWithInvalidColumnNames()
    {
        // Arrange
        XTable xTable = new XTable();

        // Act & Assert
        Assert.Throws<JsonException>(() => xTable.Put(0, "1-Invalid-Column", XValue.Create(XType.Long, 42)));
    }

    [Test]
    public void AddAndRetrieveRowsWithDifferentDataTypesInCells()
    {
        // Arrange
        XTable xTable = new XTable();
        xTable.Put(0, "Column1", XValue.Create(XType.Long, 42));
        xTable.Put(0, "Column2", XValue.Create(XType.String, "Value"));

        // Act & Assert
        Assert.That(xTable.Get(0, "Column1").Value, Is.EqualTo(42));
        Assert.That(xTable.Get(0, "Column2").Value, Is.EqualTo("Value"));
    }

    [Test]
    public void AddAndRetrieveRowsInMultiThreadedEnvironment()
    {
        // This test should ensure thread safety when adding and retrieving rows concurrently.
        // You can use multiple threads to simultaneously add and get rows in the table
        // and assert that the table remains consistent after all threads have completed.
        // Be sure to use proper synchronization mechanisms (e.g., locks) to ensure thread safety.
    }

    [Test]
    public void AddRowsWithSameColumnNamesButDifferentDataTypes()
    {
        // Arrange
        XTable xTable = new XTable();
        xTable.Put(0, "Column1", XValue.Create(XType.Long, 42));

        // Act & Assert
        Assert.Throws<ArgumentException>(() => xTable.Put(1, "Column1", XValue.Create(XType.String, "Value")));
    }

    [Test]
    public void AddAndRetrieveRowsWithEmptyValues()
    {
        // Arrange
        XTable xTable = new XTable();
        xTable.Put(0, "Column1", XValue.Create(XType.Long, null));

        // Act & Assert
        Assert.IsNull(xTable.Get(0, "Column1").Value);
    }

    [Test]
    public void AddAndRetrieveRowsWithLargeNumberOfColumnsAndRows()
    {
        // This test should check the performance of adding and retrieving rows
        // when there are a large number of columns and rows in the table.
        // You can add, retrieve, and assert a large number of rows and columns.
        // Measure the execution time to ensure it's within acceptable limits.
    }
}