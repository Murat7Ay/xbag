using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using X.Api.CuisineRecipes;
using XDataAccess;
using XProbabilisticToolkit;

namespace X.Api;

public static class RegisterUtilities
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
        RegisterEntityRoutes<EntityExample>(app);
        RegisterEntityRoutes<Recipe>(app);
        RegisterEntityRoutes<DataHolder>(app);

        
        app.MapPost("/BloomFilter/Add", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.AddItem(arguments.Key, arguments.Item));
        app.MapPost("/BloomFilter/Cardinality", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.Cardinality(arguments.Key));
        app.MapPost("/BloomFilter/Exists", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.Exists(arguments.Key, arguments.Items));
        app.MapPost("/BloomFilter/Info", (IBloomFilter bloomFilter, [FromBody] BloomFilterArguments arguments) => bloomFilter.Info(arguments.Key));
    }
   private static void RegisterEntityRoutes<TEntity>(WebApplication app) where TEntity :  Entity<TEntity>
    {
        app.MapPut($"/{typeof(TEntity).Name}",
            (IRepository<TEntity> repository, TEntity entity) => repository.InsertAsync(entity));
        app.MapPost($"/{typeof(TEntity).Name}",
            (IRepository<TEntity> repository, TEntity entity) => repository.UpdateAsync(entity));
        app.MapDelete($"/{typeof(TEntity).Name}",
            (IRepository<TEntity> repository, [FromBody] TEntity entity) => repository.DeleteAsync(entity));
        app.MapGet($"/{typeof(TEntity).Name}", (IRepository<TEntity> repository) => repository.GetListAsync());
        app.MapGet($"/{typeof(TEntity).Name}/IndexInfo", (IRepository<TEntity> repository) => repository.GetIndexInfo());
        app.MapGet($"/{typeof(TEntity).Name}/{{id}}/history",
            (IRepository<TEntity> repository, string id) => repository.GetHistoryAsync(id));
        app.MapGet($"/{typeof(TEntity).Name}/{{id}}",
            (IRepository<TEntity> repository, string id) => repository.FindByIdAsync(id));
        app.MapPost($"/{typeof(TEntity).Name}/Paged",
            (IRepository<TEntity> repository, [FromBody] DataSourceRequest request) =>
                repository.GetPagedListAsync(request));
    }
}