using StackExchange.Redis;

namespace XProbabilisticToolkit;

public class SimpleHyperLogLog : ISimpleHyperLogLog
{
    private readonly string _hyperLogLogKey;
    private readonly IDatabase _database;

    public SimpleHyperLogLog(String hyperLogLogKey, IDatabase database)
    {
        _hyperLogLogKey = hyperLogLogKey;
        _database = database;
    }

    public void Add(IEnumerable<string> elements)
    {
        var parameters = new List<object> { _hyperLogLogKey };
        parameters.AddRange(elements);
        _database.Execute("PFADD", parameters);
    }

    public void Delete()
    {
        _database.Execute("DEL", new List<object>() { _hyperLogLogKey });
    }

    public long EstimateCardinality()
    {
        return (long)_database.Execute("PFCOUNT", new List<object>() { _hyperLogLogKey });
    }
}