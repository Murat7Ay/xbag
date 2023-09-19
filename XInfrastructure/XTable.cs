using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace XInfrastructure;

public class XTable
{
    private readonly ConcurrentDictionary<int, XBag> _rows;
    public ICollection<int> RowKeys => _rows.Keys;
    public XTable()
    {
        _rows = new ConcurrentDictionary<int, XBag>();
    }

    public XBag RowToBag(int rowIndex)
    {
        return _rows.TryGetValue(rowIndex, out var bag) ? bag : new XBag();
    }

    public bool RemoveRow(int rowIndex)
    {
        return _rows.TryRemove(rowIndex, out var bag);
    }
    public void BagToRow(int rowIndex, XBag xBag)
    {
        _rows[rowIndex] = xBag;
    }

    public void Put(int rowIndex, string columnKey, XValue value)
    {
        if (rowIndex < 0)
            throw new ArgumentOutOfRangeException();
        
        if (!Utility.IsValidJsonPropertyName(columnKey))
            throw new JsonException($"{columnKey} invalid key");
        if (_rows.TryGetValue(rowIndex, out var xBag))
        {
            xBag.Put(columnKey, value);
        }
        else
        {
            XBag bag = new XBag();
            bag.Put(columnKey, value);
            _rows[rowIndex] = bag;
        }
    }

    public XValue Get(int rowIndex, string columnKey)
    {
        return _rows[rowIndex].Get(columnKey);
    }
}