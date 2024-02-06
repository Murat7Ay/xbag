using StackExchange.Redis;

namespace XProbabilisticToolkit;

public class BloomFilter : IBloomFilter
{
    private readonly IDatabase _database;

    public BloomFilter(IDatabase database)
    {
        _database = database;
    }

    public int AddItem(string key, string item)
    {
        return (int)_database.Execute("BF.ADD", new List<object>() { key, item });
    }

    public IDictionary<string, int> AddItems(string key, ISet<string> items)
    {
        throw new NotImplementedException();
    }

    public int Cardinality(string key)
    {
        return (int)_database.Execute("BF.CARD", new List<object>() { key });
    }

    public int Exists(string key, string item)
    {
        return (int)_database.Execute("BF.EXISTS", new List<object>() { key, item });
    }

    public IDictionary<string, int> Exists(string key, ISet<string> items)
    {
        Dictionary<string, int> values = new Dictionary<string, int>();
        List<object> arguments = new List<object>() { key };
        arguments.AddRange(items);
        RedisResult result = _database.Execute("BF.MEXISTS", arguments);
        if (result.Type != ResultType.MultiBulk) return values;
        var resultSet = (RedisResult[])result;
        arguments.RemoveAt(0);
        for (int i = 0; i < arguments.Count; i++)
        {
            values.Add((string)arguments[i], (int)resultSet[i]);
        }

        return values;
    }

    public IDictionary<string, string> Info(string key)
    {
        RedisResult commandResult = _database.Execute("BF.INFO", new List<object>() { key });
        var result = new Dictionary<string, string>();
        if (commandResult.Type == ResultType.MultiBulk)
        {
            var infoPairs = (RedisResult[])commandResult;
            for (int i = 0; i < infoPairs.Length; i += 2)
            {
                string attrName = (string)infoPairs[i];
                RedisResult attrValue  = infoPairs[i + 1];
                result.Add(attrName, (string)attrValue );
            }
        }
        return result;
    }

    public IDictionary<string, int> Insert(BloomFilterArguments bloomFilterArguments)
    {
        List<object> arguments = new List<object>();
        arguments.Add(bloomFilterArguments.Key);
        if (bloomFilterArguments.Capacity.HasValue)
        {
            arguments.Add("CAPACITY");
            arguments.Add(bloomFilterArguments.Capacity);
        }
        
        if (bloomFilterArguments.Error.HasValue)
        {
            arguments.Add("ERROR");
            arguments.Add(bloomFilterArguments.Error);
        }
        
        if (bloomFilterArguments.Expansion.HasValue)
        {
            arguments.Add("EXPANSION");
            arguments.Add(bloomFilterArguments.Expansion);
        }
        if (bloomFilterArguments.NoCreate)
        {
            arguments.Add("NOCREATE");
        }
        if (bloomFilterArguments.NonScaling)
        {
            arguments.Add("NONSCALING");
        }
        
        if (bloomFilterArguments.Items != null)
        {
            arguments.Add("ITEMS");
            arguments.AddRange(bloomFilterArguments.Items);
        }

        var result = _database.Execute("BF.INSERT", arguments);

        return null;
    }
}