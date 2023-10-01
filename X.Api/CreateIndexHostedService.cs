using Redis.OM;
using Redis.OM.Contracts;
using XDataAccess;

namespace X.Api;

public class CreateIndexHostedService : IHostedService
{
    private readonly RedisConnectionProvider _provider;

    public CreateIndexHostedService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var abs = typeof(Entity<>);
        var entityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(assembly => assembly.GetTypes())
            .Where(x => x.IsClass && !x.IsAbstract && x.IsInheritedFrom(abs))
            .ToList();
        IRedisConnection connection = _provider.Connection;
        foreach (Type entity in entityTypes)
        {
            await connection.CreateIndexAsync(entity);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}