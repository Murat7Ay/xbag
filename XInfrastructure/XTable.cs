using System.Collections.ObjectModel;
using System.Text.Json;

namespace XInfrastructure;

public class XTable
{
    private readonly List<string> _columnKeys;
    private readonly List<List<IXData>> _rows;
    private readonly Dictionary<string, XType> _columnTypeMap;

    public int RowCount => _rows.Count;

    public IReadOnlyList<string> GetColumns()
    {
        return _columnKeys.AsReadOnly();
    }


    public XTable()
    {
        _columnKeys = new List<string>();
        _rows = new List<List<IXData>>();
        _columnTypeMap = new Dictionary<string, XType>();
    }

    public XBag RowToBag(int rowIndex)
    {
        XBag xBag = new XBag();
        int length = _columnKeys.Count;
        for (int i = 0; i < length; i++)
        {
            xBag.Put(_columnKeys[i], _rows[rowIndex][i]);
        }
        return xBag;
    }

    public void BagToRow(int rowIndex, XBag xBag)
    {
        ReadOnlyDictionary<string, IXData> readOnlyDictionary = xBag.GetReadOnlyDictionary();
        foreach (KeyValuePair<string, IXData> kv in readOnlyDictionary)
        {
            Put(rowIndex, kv.Key, kv.Value);
        }
    }

    public void Put(int rowIndex, string columnKey, IXData value)
    {
        if (!Utility.IsValidJsonPropertyName(columnKey))
            throw new JsonException($"{columnKey} invalid key");
        var columnIndex = ArrangeColumnIndex(rowIndex, columnKey, value);
        _rows[rowIndex][columnIndex] = value;
    }

    public IXData Get(int rowIndex, string columnKey)
    {
        int columnIndex = _columnKeys.IndexOf(columnKey);
        if (columnIndex == -1)
        {
            return XValue.Create(XType.None, null);
        }

        return rowIndex >= _rows.Count ? XValue.Create(XType.None, null) : _rows[rowIndex][columnIndex];
    }

    private int ArrangeColumnIndex(int rowIndex, string columnKey, IXData xData)
    {
        if (_columnTypeMap.TryGetValue(columnKey, out XType xType))
        {
            if (xType != xData.XType)
            {
                throw new ArgumentException();
            }
        }
        else
        {
            _columnTypeMap.Add(columnKey, xData.XType);
        }
        int columnIndex = _columnKeys.IndexOf(columnKey);
        if (columnIndex == -1)
        {
            _columnKeys.Add(columnKey);
            columnIndex = _columnKeys.Count - 1;
            for (int i = 0; i < _rows.Count; i++)
            {
                _rows[i].Add(XValue.Create(xData.XType, null));
            }
        }

        while (rowIndex >= _rows.Count)
        {
            List<IXData> row = new List<IXData>();
            for (int i = 0; i < _columnKeys.Count; i++)
            {
                row.Add(XValue.Create(_columnTypeMap[_columnKeys[i]], null));
            }
            _rows.Add(row);
        }
        return columnIndex;
    }
}