namespace XDataAccess;

public interface IFilterCondition
{
    public void SetFilter(string key, bool value);
    public bool GetFilter(string key);
}