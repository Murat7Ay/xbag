using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using XDataAccess;
using XProbabilisticToolkit;

namespace X.Api;

public static class RegisterInfrastructures
{
    public static void AddProbabilistic(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(typeof(IBloomFilter), typeof(BloomFilter));
    }

    public static void AddRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(typeof(IRepository<>), typeof(RedisRepository<>));
    }

    public static void AddApis(WebApplication app)
    {
        app.MapPut("/EntityExample", (IRepository<EntityExample> repository, EntityExample entity) => repository.InsertAsync(entity));
        app.MapPost("/EntityExample", (IRepository<EntityExample> repository, EntityExample entity) => repository.UpdateAsync(entity));
        app.MapDelete("/EntityExample", (IRepository<EntityExample> repository, [FromBody] EntityExample entity) => repository.DeleteAsync(entity));
        app.MapGet("/EntityExample", (IRepository<EntityExample> repository) => repository.GetListAsync());
        app.MapGet("/EntityExample/IndexInfo", (IRepository<EntityExample> repository) => repository.GetIndexInfo());
        app.MapGet("/EntityExample/{id}/history", (IRepository<EntityExample> repository, string id) => repository.GetHistoryAsync(id));
        app.MapGet("/EntityExample/{id}", (IRepository<EntityExample> repository, string id) => repository.FindByIdAsync(id));
        app.MapPost("/EntityExample/Paged", (IRepository<EntityExample> repository, [FromBody] DataSourceRequest request) => repository.GetPagedListAsync(request));

        app.MapPost("/EntityExample/Custom",
            (IRepository<EntityExample> repository, [FromBody] DataSourceRequest request) =>
            {
                var col = repository.GetCollection;
                var res = col.Where(x => x.IsActive && !x.IsDeleted && (x.Prop1 == "test1" || x.Prop1 != "test1"));
                return res.ToList();
            });

        app.MapPost("/BloomFilter/Add", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.AddItem(arguments.Key, arguments.Item));
        app.MapPost("/BloomFilter/Cardinality", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.Cardinality(arguments.Key));
        app.MapPost("/BloomFilter/Exists", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.Exists(arguments.Key, arguments.Items));
        app.MapPost("/BloomFilter/Info", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.Info(arguments.Key));
    }
}