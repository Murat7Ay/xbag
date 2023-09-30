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
    }
}