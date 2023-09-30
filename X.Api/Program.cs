using Redis.OM;
using Redis.OM.Contracts;
using Redis.OM.Modeling;
using StackExchange.Redis;
using XDataAccess;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton(new RedisConnectionProvider(new ConfigurationOptions
    { EndPoints = { "localhost:6379" } }));
builder.Services.AddHostedService<IndexCreationService>();
var app = builder.Build();

app.MapGet("/", async () =>
{
    var conf = new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" }
    };
    var rep = new RedisRepository<EntityExample>(conf, new AuthUser(), new Clock(), new FilterCondition());

    return await rep.GetListAsync(entity => entity.Prop1 == "1cdd129c-0e08-4024-ab19-1a20a9439e83" && entity.IsDeleted == false);
});
app.MapPost("/test", async (EntityExample entity, CancellationToken cancellationToken) =>
{
    var conf = new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" }
    };
    var rep = new RedisRepository<EntityExample>(conf, new AuthUser(), new Clock(), new FilterCondition());

    return await rep.GetListAsync(test1Entity => test1Entity.XId == "2309301");
});

app.Run();

[Document(StorageType = StorageType.Json, Prefixes = new[] { "EntityExample" })]
public class EntityExample : Entity<EntityExample>
{
    [Searchable]
    public string? Prop1 { get; set; }

    public override IList<EntityChange> GetChanges(EntityExample compare)
    {
        return new List<EntityChange>()
        {
            new EntityChange
            {
                Name = nameof(Prop1),
                NewValue = Prop1,
                OldValue = compare?.Prop1
            }
        };
    }
}


public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider _provider;

    public IndexCreationService(RedisConnectionProvider provider)
    {
        _provider = provider;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var entityTypes = System.Reflection.Assembly.GetExecutingAssembly().GetTypes()
            .Where(mytype => mytype.IsClass && mytype.GetInterfaces().Contains(typeof(Entity<>)));
        IRedisConnection connection = _provider.Connection;
        foreach (Type entity in entityTypes)
        {
            await connection.CreateIndexAsync(entity);
        }
        await _provider.Connection.CreateIndexAsync(typeof(EntityExample));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}