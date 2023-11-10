using Microsoft.AspNetCore.Mvc;
using Redis.OM;
using StackExchange.Redis;
using XDataAccess;

public static class RegisterEntities
{
    public static void AddRepositories(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton(typeof(IRepository<>), typeof(RedisRepository<>));
    }
    public static void AddApis(WebApplication app)
    {
        // app.MapPut("/rose", (IRepository<RoseEntity> repository, RoseEntity entity) => repository.Save(entity)).RequireAuthorization("root");
        // app.MapPost("/rose", (IRepository<RoseEntity> repository, RoseEntity entity) => repository.Update(entity)).RequireAuthorization("root");
        // app.MapDelete("/rose", (IRepository<RoseEntity> repository, string id) => repository.Delete(id)).RequireAuthorization("root");
        // app.MapGet("/rose", (IRepository<RoseEntity> repository) => repository.Get()).RequireAuthorization("root");
        // app.MapGet("/rose/{id}/history", (IRepository<RoseEntity> repository, string id) => repository.GetHistory(id)).RequireAuthorization("root");
        // app.MapGet("/rose/{id}", (IRepository<RoseEntity> repository, string id) => repository.FindById(id)).RequireAuthorization("root");;
        // app.MapGet("/rose/{offset}/{limit}", (IRepository<RoseEntity> repository, int offset, int limit) => repository.Get(offset, limit)).RequireAuthorization("root");
        
        app.MapPut("/EntityExample", (IRepository<EntityExample> repository, EntityExample entity) => repository.InsertAsync(entity));
        app.MapPost("/EntityExample", (IRepository<EntityExample> repository, EntityExample entity) => repository.UpdateAsync(entity));
        app.MapDelete("/EntityExample", (IRepository<EntityExample> repository, [FromBody]EntityExample entity) => repository.DeleteAsync(entity));
        app.MapGet("/EntityExample", (IRepository<EntityExample> repository) => repository.GetListAsync());
        app.MapGet("/EntityExample/IndexInfo", (IRepository<EntityExample> repository) => repository.GetIndexInfo());
        app.MapGet("/EntityExample/{id}/history", (IRepository<EntityExample> repository, string id) => repository.GetHistoryAsync(id));
        app.MapGet("/EntityExample/{id}", (IRepository<EntityExample> repository, string id) => repository.FindByIdAsync(id));
        app.MapPost("/EntityExample/Paged", (IRepository<EntityExample> repository, [FromBody]DataSourceRequest request) => repository.GetPagedListAsync(request));
        
        app.MapPost("/EntityExample/Custom",
            (IRepository<EntityExample> repository, [FromBody] DataSourceRequest request) =>
            {
                var col = repository.GetCollection;
                var res = col.Where(x => x.IsActive && !x.IsDeleted && (x.Prop1 == "test1" || x.Prop1 != "test1"));
                return res.ToList();
            });

    }
}