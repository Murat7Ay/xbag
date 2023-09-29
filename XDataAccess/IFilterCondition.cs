using System.Collections.Concurrent;

namespace XDataAccess;


public class FilterCondition : IFilterCondition
{
    private readonly ConcurrentDictionary<string, bool> _conditions;

    public FilterCondition()
    {
        _conditions = new ConcurrentDictionary<string, bool>
        {
            ["IsDeleted"] = false,
            ["IsActive"] = true
        };
    }
    public void SetFilter(string key, bool value)
    {
        
    }

    public bool GetFilter(string key)
    {
        return _conditions[key];
    }
}

public interface IFilterCondition
{
    public void SetFilter(string key, bool value);
    public bool GetFilter(string key);
}