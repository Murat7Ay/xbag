using StackExchange.Redis;

namespace XProbabilisticToolkit;

public class CuckooFilterFactory
{
    private readonly IDatabase _database;

    public CuckooFilterFactory(IDatabase database)
    {
        _database = database;
    }

    public ICuckooFilter Create(string filterName)
    {
        return new CuckooFilter(filterName, _database);
    }
}