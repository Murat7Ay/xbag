namespace XProbabilisticToolkit;

public interface IHyperLogLog
{
    Task<bool> AddAsync(string key, IEnumerable<string> values);
    Task<bool> AddAsync(string key, long ticks, IEnumerable<string> values);
    Task<long> LengthAsync(string key);
    Task<long> LengthAsync(params string[] key);
    Task MergeAsync(string destinationKey, IEnumerable<string> sourceKeys);
    Task<bool> ExpireAsync(string key, long ticks);
}