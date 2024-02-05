using StackExchange.Redis;

namespace XProbabilisticToolkit;

public class HyperLogLog : IHyperLogLog
{
    private readonly IDatabase _database;

    public HyperLogLog(IDatabase database)
    {
        _database = database;
    }

    public async Task<bool> AddAsync(string key, params string[] values)
    {
        return await _database.HyperLogLogAddAsync(key, RedisValues(values), CommandFlags.FireAndForget);
    }

    public async Task<bool> AddAsync(string key, long ticks, params string[] values)
    {
        var result = await AddAsync(key, values);
        await ExpireAsync(key, ticks);
        return result;
    }

    public async Task<long> LengthAsync(string key)
    {
        return await LengthAsync(new[] { key });
    }

    public async Task<long> LengthAsync(params string[] key)
    {
        return await _database.HyperLogLogLengthAsync(RedisKeys(key));
    }

    public async Task MergeAsync(string destinationKey, params string[] sourceKeys)
    {
        await _database.HyperLogLogMergeAsync(destinationKey, RedisKeys(sourceKeys));
    }

    public async Task<bool> ExpireAsync(string key, long ticks)
    {
        return await _database.KeyExpireAsync(key, new TimeSpan(ticks));
    }

    private static RedisValue[] RedisValues(string[] values) => values.Select(s => new RedisValue(s)).ToArray();
    private static RedisKey[] RedisKeys(string[] key) => key.Select(s => new RedisKey(s)).ToArray();
}