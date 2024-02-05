namespace XProbabilisticToolkit;

public interface IHyperLogLog
{
    Task<bool> AddAsync(string key, params string[] values);
    Task<bool> AddAsync(string key, long ticks, params string[] values);
    Task<long> LengthAsync(string key);
    Task<long> LengthAsync(params string[] key);
    Task MergeAsync(string destinationKey, params string[] sourceKeys);
    Task<bool> ExpireAsync(string key, long ticks);
}