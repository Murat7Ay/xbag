using StackExchange.Redis;

namespace XProbabilisticToolkit;

public class CuckooFilter : ICuckooFilter
{
    private readonly string _filterName;
    private readonly IDatabase _database;

    public CuckooFilter(string filterName, IDatabase database)
    {
        _filterName = filterName;
        _database = database;
    }
    public bool Add(string element)
    {
        return (int)_database.Execute("CF.ADD", new List<object>() { _filterName, element }) == 1;
    }

    public bool AddIfNotExists(string element)
    {
        return (int)_database.Execute("CF.ADDNX", new List<object>() { _filterName, element }) == 1;
    }

    public int Add(IEnumerable<string> elements)
    {
        var parameters = new List<object>() { _filterName };
        parameters.AddRange(elements);
        return (int)_database.Execute("CF.ADDNX", parameters);
    }

    public int AddIfNotExists(IEnumerable<string> elements)
    {
        throw new NotImplementedException();
    }

    public bool MightContain(string element)
    {
        throw new NotImplementedException();
    }

    public IDictionary<string, bool> MightContain(IEnumerable<string> elements)
    {
        throw new NotImplementedException();
    }

    public bool Remove(string element)
    {
        throw new NotImplementedException();
    }

    public bool CreateFilter(int capacity, string[] fpFunctionNames)
    {
        throw new NotImplementedException();
    }

    public IDictionary<string, string> Info()
    {
        throw new NotImplementedException();
    }

    public long EstimatedCount()
    {
        throw new NotImplementedException();
    }
}