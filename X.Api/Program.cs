using Redis.OM;
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
    var rep = new RedisRepository<Test1Entity>(conf, new AuthUser(), new Clock(), new FilterCondition());
    var t = new Test1Entity();
    t.Prop1 = "murat";
    await rep.InsertAsync(t, default);
    return await rep.GetListAsync(default);
});
app.MapPost("/test", async (Test1Entity entity, CancellationToken cancellationToken) =>
{
    var conf = new ConfigurationOptions
    {
        EndPoints = { "localhost:6379" }
    };
    var rep = new RedisRepository<Test1Entity>(conf, new AuthUser(), new Clock(), new FilterCondition());

    return await rep.InsertAsync(entity, cancellationToken);
});

app.Run();

[Document(StorageType = StorageType.Json, Prefixes = new[] { "test11" })]
public class Test1Entity : IEntity<Test1Entity>
{
    public string? Prop1 { get; set; }

    [RedisIdField] [Indexed]
    public string? Id { get; set; }
    public string? XId { get; set; }
    [Indexed]
    public bool IsDeleted { get; set; }
    [Indexed]
    public bool IsActive { get; set; }
    [Indexed]
    public string? TraceId { get; set; }
    public string? DeletedBy { get; set; }
    public DateTime? DeleteDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public string? ModifiedBy { get; set; }
    public DateTime CreateDate { get; set; }
    public string? CreatedBy { get; set; }
    public int EntityVersion { get; set; }
    public string? Ip { get; set; }
    public string? Host { get; set; }

    public object?[] GetKeys()
    {
        return new object?[] { Id };
    }

    public IList<EntityChange> GetChanges(Test1Entity compare)
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

    public override string ToString()
    {
        return $"[ENTITY: {GetType().Name}] Id = {Id}";
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
        await _provider.Connection.CreateIndexAsync(typeof(Test1Entity));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}