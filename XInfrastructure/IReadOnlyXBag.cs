using System.Collections.ObjectModel;

namespace XInfrastructure;

public interface IReadOnlyXBag
{
    bool ContainsKey(string key);
    XValue Get(string key);
    XValue GetWithDefault(string key, XValue defaultValue);
    ReadOnlyDictionary<string, XValue> GetReadOnlyDictionary();
}