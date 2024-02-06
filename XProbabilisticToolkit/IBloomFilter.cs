namespace XProbabilisticToolkit;

public interface IBloomFilter
{
    int AddItem(string key, string item);
    IDictionary<string, int> AddItems(string key, ISet<string> items);
    int Cardinality(string key);
    int Exists(string key, string item);
    IDictionary<string, int> Exists(string key, ISet<string> items);
    IDictionary<string, string> Info(string key);
    IDictionary<string, int> Insert(BloomFilterArguments bloomFilterArguments);
}