using StackExchange.Redis;

namespace XProbabilisticToolkit;

public class ProbabilisticFactory : IProbabilisticFactory
{
    private readonly IDatabase _database;

    public ProbabilisticFactory(IDatabase database)
    {
        _database = database;
    }

    public ICuckooFilter CreateCuckooFilter(string filterName)
    {
        return new CuckooFilter(filterName, _database);
    }

    public IBloomFilter CreateBloomFilter(string filterName)
    {
        return new BloomFilter(filterName, _database);
    }

    public ISimpleHyperLogLog CreateSimpleHyperLogLog(string hyperLogLogKey)
    {
        return new SimpleHyperLogLog(hyperLogLogKey, _database);
    }
}