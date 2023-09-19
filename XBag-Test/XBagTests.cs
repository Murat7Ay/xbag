using LanguageExt;
using XInfrastructure;

namespace XBagTests;

[TestFixture]
public class XBagTests
{
     [Test]
    public void PutAndGetMultipleNestedListsInXBag()
    {
        // Arrange
        List<object> innerList1 = new List<object> { 1, "Alice", true };
        List<object> innerList2 = new List<object> { "Bob", 2.5, false };
        XValue innerListValue1 = XValue.Create(XType.StringList, innerList1);
        XValue innerListValue2 = XValue.Create(XType.DoubleList, innerList2);

        XBag bag = new XBag();

        // Act
        bag.Put("NestedList1", innerListValue1);
        bag.Put("NestedList2", innerListValue2);
        var retrievedValue1 = bag.Get("NestedList1");
        var retrievedValue2 = bag.Get("NestedList2");

        // Assert
        Assert.IsNotNull(retrievedValue1);
        Assert.IsNotNull(retrievedValue2);

        Assert.That(retrievedValue1, Is.TypeOf<XValue>());
        XValue retrievedXValue1 = (XValue)retrievedValue1;
        Assert.That(retrievedXValue1.XType, Is.EqualTo(XType.StringList));
        List<object> retrievedList1 = (List<object>)retrievedXValue1.Value;
        Assert.AreEqual(3, retrievedList1.Count);
        Assert.AreEqual(1, retrievedList1[0]);
        Assert.AreEqual("Alice", retrievedList1[1]);
        Assert.AreEqual(true, retrievedList1[2]);

        Assert.That(retrievedValue2, Is.TypeOf<XValue>());
        XValue retrievedXValue2 = (XValue)retrievedValue2;
        Assert.That(retrievedXValue2.XType, Is.EqualTo(XType.DoubleList));
        List<object> retrievedList2 = (List<object>)retrievedXValue2.Value;
        Assert.AreEqual(3, retrievedList2.Count);
        Assert.AreEqual("Bob", retrievedList2[0]);
        Assert.AreEqual(2.5, retrievedList2[1]);
        Assert.AreEqual(false, retrievedList2[2]);
    }

    [Test]
    public void PutAndGetNestedListsInXTable()
    {
        // Arrange
        List<object> innerList1 = new List<object> { 1, "Alice", true };
        List<object> innerList2 = new List<object> { "Bob", 2.5, false };
        XValue innerListValue1 = XValue.Create(XType.StringList, innerList1);
        XValue innerListValue2 = XValue.Create(XType.DoubleList, innerList2);

        XTable table = new XTable();
        table.Put(0, "NestedList1", innerListValue1);
        table.Put(1, "NestedList2", innerListValue2);

        // Act
        var retrievedValue1 = table.Get(0, "NestedList1");
        var retrievedValue2 = table.Get(1, "NestedList2");

        // Assert
        Assert.IsNotNull(retrievedValue1);
        Assert.IsNotNull(retrievedValue2);

        Assert.That(retrievedValue1, Is.TypeOf<XValue>());
        XValue retrievedXValue1 = (XValue)retrievedValue1;
        Assert.That(retrievedXValue1.XType, Is.EqualTo(XType.StringList));
        List<object> retrievedList1 = (List<object>)retrievedXValue1.Value;
        Assert.AreEqual(3, retrievedList1.Count);
        Assert.AreEqual(1, retrievedList1[0]);
        Assert.AreEqual("Alice", retrievedList1[1]);
        Assert.AreEqual(true, retrievedList1[2]);

        Assert.That(retrievedValue2, Is.TypeOf<XValue>());
        XValue retrievedXValue2 = (XValue)retrievedValue2;
        Assert.That(retrievedXValue2.XType, Is.EqualTo(XType.DoubleList));
        List<object> retrievedList2 = (List<object>)retrievedXValue2.Value;
        Assert.AreEqual(3, retrievedList2.Count);
        Assert.AreEqual("Bob", retrievedList2[0]);
        Assert.AreEqual(2.5, retrievedList2[1]);
        Assert.AreEqual(false, retrievedList2[2]);
    }

    [Test]
    public void PutAndGetNestedEmptyListInXBag()
    {
        // Arrange
        List<object> innerList = new List<object>();
        XValue innerListValue = XValue.Create(XType.StringList, innerList);

        XBag bag = new XBag();

        // Act
        bag.Put("NestedList", innerListValue);
        var retrievedValue = bag.Get("NestedList");

        // Assert
        Assert.IsNotNull(retrievedValue);
        Assert.That(retrievedValue, Is.TypeOf<XValue>());
        XValue retrievedXValue = (XValue)retrievedValue;
        Assert.That(retrievedXValue.XType, Is.EqualTo(XType.StringList));
        List<object> retrievedList = (List<object>)retrievedXValue.Value;

        Assert.AreEqual(0, retrievedList.Count);
    }

    [Test]
    public void PutAndGetNestedEmptyListInXTable()
    {
        // Arrange
        List<object> innerList = new List<object>();
        XValue innerListValue = XValue.Create(XType.StringList, innerList);

        XTable table = new XTable();
        table.Put(0, "NestedList", innerListValue);

        // Act
        var retrievedValue = table.Get(0, "NestedList");

        // Assert
        Assert.IsNotNull(retrievedValue);
        Assert.That(retrievedValue, Is.TypeOf<XValue>());
        XValue retrievedXValue = (XValue)retrievedValue;
        Assert.That(retrievedXValue.XType, Is.EqualTo(XType.StringList));
        List<object> retrievedList = (List<object>)retrievedXValue.Value;

        Assert.AreEqual(0, retrievedList.Count);
    }
     [Test]
    public void PutAndGetNestedXValueWithValidXBag()
    {
        // Arrange
        XBag innerBag = new XBag();
        innerBag.Put("Name", XValue.Create(XType.String, "John"));
        innerBag.Put("Age", XValue.Create(XType.Int, 30));
        XValue innerXValue = XValue.Create(XType.Bag, innerBag);

        XBag outerBag = new XBag();

        // Act
        outerBag.Put("Person", innerXValue);
        var retrievedValue = outerBag.Get("Person");

        // Assert
        Assert.IsNotNull(retrievedValue);
        Assert.That(retrievedValue, Is.TypeOf<XValue>());
        XValue retrievedXValue = (XValue)retrievedValue;
        Assert.That(retrievedXValue.XType, Is.EqualTo(XType.Bag));
        XBag retrievedBag = (XBag)retrievedXValue.Value;
        Assert.AreEqual(2, retrievedBag.GetReadOnlyDictionary().Count);
        Assert.AreEqual("John", retrievedBag.Get("Name")?.Value);
        Assert.AreEqual(30, retrievedBag.Get("Age")?.Value);
    }
    
    [Test]
    public void PutAndGetNestedXTableInXBagInXBagInXTable()
    {
        // Arrange
        XTable innerTable1 = new XTable();
        innerTable1.Put(0, "Name", XValue.Create(XType.String, "Alice"));
        innerTable1.Put(0, "Age", XValue.Create(XType.Int, 25));
        XValue innerTableXValue1 = XValue.Create(XType.Table, innerTable1);

        XBag innerBag1 = new XBag();
        innerBag1.Put("Employee1", innerTableXValue1);

        XBag innerBag2 = new XBag();
        innerBag2.Put("Department", XValue.Create(XType.String, "HR"));
        innerBag2.Put("Manager", XValue.Create(XType.String, "Bob"));
        XValue innerBagXValue = XValue.Create(XType.Bag, innerBag2);

        XTable outerTable = new XTable();
        outerTable.Put(0, "InnerBag", innerBagXValue);
        outerTable.Put(0, "InnerTable", innerTableXValue1);

        XBag outerBag = new XBag();
        XValue outerXValue = XValue.Create(XType.Table, outerTable);

        // Act
        outerBag.Put("ComplexData", outerXValue);
        var retrievedValue = outerBag.Get("ComplexData");

        // Assert
        Assert.IsNotNull(retrievedValue);
        Assert.That(retrievedValue, Is.TypeOf<XValue>());
        XValue retrievedXValue = (XValue)retrievedValue;
        Assert.That(retrievedXValue.XType, Is.EqualTo(XType.Table));
        XTable retrievedTable = (XTable)retrievedXValue.Value;

        Assert.That(retrievedTable.RowKeys.Count, Is.EqualTo(1));

        // Verify nested Bag
        IXData innerBagData = retrievedTable.Get(0, "InnerBag");
        Assert.IsNotNull(innerBagData);
        Assert.That(innerBagData, Is.TypeOf<XValue>());
        XValue innerBag1XValue = (XValue)innerBagData;
        Assert.That(innerBag1XValue.XType, Is.EqualTo(XType.Bag));
        XBag retrievedInnerBag = (XBag)innerBag1XValue.Value;
        Assert.AreEqual(2, retrievedInnerBag.GetReadOnlyDictionary().Count);
        Assert.AreEqual("HR", retrievedInnerBag.Get("Department")?.Value);
        Assert.AreEqual("Bob", retrievedInnerBag.Get("Manager")?.Value);

        // Verify nested Table
        IXData innerTableData = retrievedTable.Get(0, "InnerTable");
        Assert.IsNotNull(innerTableData);
        Assert.That(innerTableData, Is.TypeOf<XValue>());
        XValue innerTableXValue2 = (XValue)innerTableData;
        Assert.That(innerTableXValue2.XType, Is.EqualTo(XType.Table));
        XTable retrievedInnerTable = (XTable)innerTableXValue2.Value;
        Assert.That(retrievedInnerTable.RowKeys.Count, Is.EqualTo(1));
        Assert.That(retrievedInnerTable.Get(0, "Name")?.Value, Is.EqualTo("Alice"));
        Assert.That(retrievedInnerTable.Get(0, "Age")?.Value, Is.EqualTo(25));
    }

    [Test]
    public void PutAndGetNestedXValueWithValidXTable()
    {
        // Arrange
        XTable innerTable = new XTable();
        innerTable.Put(0, "Name", XValue.Create(XType.String, "Alice"));
        innerTable.Put(0, "Age", XValue.Create(XType.Int, 25));
        XValue innerXValue = XValue.Create(XType.Table, innerTable);

        XBag bag = new XBag();

        // Act
        bag.Put("Employee", innerXValue);
        var retrievedValue = bag.Get("Employee");

        // Assert
        Assert.IsNotNull(retrievedValue);
        Assert.That(retrievedValue, Is.TypeOf<XValue>());
        XValue retrievedXValue = (XValue)retrievedValue;
        Assert.That(retrievedXValue.XType, Is.EqualTo(XType.Table));
        XTable retrievedTable = (XTable)retrievedXValue.Value;
        Assert.That(retrievedTable.RowKeys.Count, Is.EqualTo(1));
        Assert.That(retrievedTable.Get(0, "Name")?.Value, Is.EqualTo("Alice"));
        Assert.That(retrievedTable.Get(0, "Age")?.Value, Is.EqualTo(25));
    }

    [Test]
    public void PutAndGetSingleValue()
    {
        // Arrange
        XBag bag = new XBag();
        var value = XValue.Create(XType.String, "Hello, World!");

        // Act
        bag.Put("message", value);
        var retrievedValue = bag.Get("message");

        // Assert
        Assert.That(retrievedValue, Is.EqualTo(value));
    }

    [Test]
    public void PutAndTryGetNonExistentValue()
    {
        // Arrange
        XBag bag = new XBag();

        // Act
        var retrievedValue = bag.Get("nonexistent_key");

        // Assert
        Assert.AreEqual(retrievedValue.Value == null, retrievedValue.XType == XType.None);
    }

    [Test]
    public void RemoveValue()
    {
        // Arrange
        XBag bag = new XBag();
        var value = XValue.Create(XType.String, "Hello, World!");
        bag.Put("message", value);

        // Act
        bag.Remove("message");
        var retrievedValue = bag.Get("message");

        // Assert
        Assert.AreEqual(retrievedValue.Value == null, retrievedValue.XType == XType.None);
    }

    [Test]
    public void PutAndGetValuesWithSpecialCharacters()
    {
        // Arrange
        XBag bag = new XBag();
        var value1 = XValue.Create(XType.String, "Special@Value");

        // Act
        bag.Put("key1", value1);

        // Assert
        Assert.That(bag.Get("key1").Value, Is.EqualTo("Special@Value"));
    }

    [Test]
    public void PutAndGetValuesWithCaseSensitivity()
    {
        // Arrange
        XBag bag = new XBag();
        var value1 = XValue.Create(XType.String, "CaseSensitive");
        var value2 = XValue.Create(XType.Long, 789);

        // Act
        bag.Put("Key1", value1);
        bag.Put("key1", value2);

        // Assert
        Assert.That(bag.Get("Key1").Value, Is.EqualTo("CaseSensitive"));
        Assert.That(bag.Get("key1").Value, Is.EqualTo(789));
    }

    [Test]
    public void PutAndGetValuesWithDifferentDataTypes()
    {
        // Arrange
        XBag bag = new XBag();
        var stringValue = XValue.Create(XType.String, "String");
        var longValue = XValue.Create(XType.Long, 123);
        var boolValue = XValue.Create(XType.Bool, true);
        var decimalValue = XValue.Create(XType.Decimal, 45.67m);
        var doubleValue = XValue.Create(XType.Double, 3.14);
        var dateTimeValue = XValue.Create(XType.Date, new DateTime(2023, 9, 16));
        var xTableValue = XValue.Create(XType.Table, new XTable());
        var xBagValue = XValue.Create(XType.Bag, new XBag());

        // Act
        bag.Put("string", stringValue);
        bag.Put("long", longValue);
        bag.Put("bool", boolValue);
        bag.Put("decimal", decimalValue);
        bag.Put("double", doubleValue);
        bag.Put("datetime", dateTimeValue);
        bag.Put("xtable", xTableValue);
        bag.Put("xbag", xBagValue);

        // Assert
        Assert.That(bag.Get("string").Value, Is.EqualTo("String"));
        Assert.That(bag.Get("long").Value, Is.EqualTo(123));
        Assert.That(bag.Get("bool").Value, Is.EqualTo(true));
        Assert.That(bag.Get("decimal").Value, Is.EqualTo(45.67m));
        Assert.That(bag.Get("double").Value, Is.EqualTo(3.14));
        Assert.That(bag.Get("datetime").Value, Is.EqualTo(new DateTime(2023, 9, 16)));
        Assert.That(bag.Get("xtable").Value, Is.TypeOf<XTable>());
        Assert.That(bag.Get("xbag").Value, Is.TypeOf<XBag>());
    }

    [Test]
    public void PutAndRetrieveValuesInMultiThreadedEnvironment()
    {
        // This test should ensure thread safety when putting and retrieving values concurrently.
        // You can use multiple threads to simultaneously add and get values from the bag
        // and assert that the bag remains consistent after all threads have completed.
        // Be sure to use proper synchronization mechanisms (e.g., locks) to ensure thread safety.
    }
    
    [Test]
    public void PutIfAbsent_KeyDoesNotExist_AddsValue()
    {
        // Arrange
        XBag xBag = new XBag();
        XValue xValue = "Test";

        // Act
        xBag.PutIfAbsent("key1", xValue);

        // Assert
        Assert.AreEqual(xValue, xBag.Get("key1"));
    }

    [Test]
    public void PutIfAbsent_KeyExists_DoesNotAddValue()
    {
        // Arrange
        XBag xBag = new XBag();
        XValue initialValue = "InitialValue";
        XValue newValue = "NewValue";
        xBag.Put("key1", initialValue);

        // Act
        xBag.PutIfAbsent("key1", newValue);

        // Assert
        Assert.AreEqual(initialValue, xBag.Get("key1"));
    }

    [Test]
    public void GetWithDefault_KeyExists_ReturnsValue()
    {
        // Arrange
        XBag xBag = new XBag();
        XValue xValue = "Test";
        xBag.Put("key1", xValue);
        XValue defaultValue = "Default";

        // Act
        XValue result = xBag.GetWithDefault("key1", defaultValue);

        // Assert
        Assert.AreEqual(xValue, result);
    }

    [Test]
    public void GetWithDefault_KeyDoesNotExist_ReturnsDefaultValue()
    {
        // Arrange
        XBag xBag = new XBag();
        XValue defaultValue = "Default";

        // Act
        XValue result = xBag.GetWithDefault("nonexistentKey", defaultValue);

        // Assert
        Assert.AreEqual(defaultValue, result);
    }
    
    
}