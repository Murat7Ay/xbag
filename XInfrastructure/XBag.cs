using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace XInfrastructure;

using System.Collections.Generic;

public class XBag : IReadOnlyXBag
{
    private readonly ConcurrentDictionary<string, XValue> _data = new();

    public void Put(string key, XValue value)
    {
        if (!Utility.IsValidJsonPropertyName(key))
            throw new JsonException($"{key} invalid key");
        _data[key] = value;
    }

    public void PutIfAbsent(string key, XValue value)
    {
        if(!_data.ContainsKey(key))
            Put(key, value);
    }

    public bool ContainsKey(string key)
    {
        return _data.ContainsKey(key);
    }

    public XValue Get(string key)
    {
        return _data.TryGetValue(key, out var value) ? value : XValue.Create(XType.None, null);
    }

    public XValue GetWithDefault(string key, XValue defaultValue)
    {
        return _data.TryGetValue(key, out var value) ? value : defaultValue;
    }

    public ReadOnlyDictionary<string, XValue> GetReadOnlyDictionary()
    {
        return _data.AsReadOnly();
    }

    public bool Remove(string key)
    {
        return _data.Remove(key, out _);
    }
}