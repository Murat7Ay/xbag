using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Redis.OM;
using StackExchange.Redis;
using X.Api;
using XDataAccess;
using XProbabilisticToolkit;


var builder = WebApplication.CreateBuilder(args);
string redisPort = builder.Configuration["Redis:Port"]!;
var secretKey = ApiSettings.GenerateSecretByte();

builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("reader", policy => policy.RequireRole("reader"));
    options.AddPolicy("moderator", policy => policy.RequireRole("moderator"));
    options.AddPolicy("root", policy => policy.RequireRole("root"));
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Internal Generator Apis", Version = "v1" });
    OpenApiSecurityScheme securityDefinition = new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        BearerFormat = "JWT",
        Scheme = "Bearer",
        Description = "Specify the authorization token",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
    };
    c.AddSecurityDefinition("Bearer", securityDefinition);
    OpenApiSecurityRequirement securityRequirement = new OpenApiSecurityRequirement();
    OpenApiSecurityScheme secondSecurityDefinition = new OpenApiSecurityScheme
    {
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    securityRequirement.Add(secondSecurityDefinition, new string[] { });
    c.AddSecurityRequirement(securityRequirement);
});
builder.Services.AddSingleton(new RedisConnectionProvider(new ConfigurationOptions
    { EndPoints = { redisPort } }));
builder.Services.AddSingleton<IDatabase>(cfg =>
{
    IConnectionMultiplexer multiplexer = ConnectionMultiplexer.Connect(redisPort);
    return multiplexer.GetDatabase();
});
builder.Services.AddTransient<IClock>(_ => new Clock());
builder.Services.AddTransient<IFilterCondition>(_ => new FilterCondition());
builder.Services.AddTransient<IAuthUser>(_ => new AuthUser());
builder.Services.AddHostedService<CreateIndexHostedService>();
builder.Services.AddSingleton<TokenService>();
builder.Services.AddHttpContextAccessor();
RegisterUtilities.AddRepositories(builder);
RegisterUtilities.AddProbabilistic(builder);
var app = builder.Build();
app.UseAuthentication();
app.UseAuthorization();
app.UseSwagger();
app.UseSwaggerUI();
RegisterUtilities.AddApis(app);

app.MapPut("/user", SaveUser());
app.MapPost("/login", LoginUser());
app.Run();

Func<TokenService, IRepository<UserEntity>, UserEntity, Task<string>> LoginUser()
{
    return async (service, userRepository, userModel) =>
    {
        var hashed = service.GetPasswordHash(userModel.Password);
        var use1r = await userRepository.FindByIdAsync("01HBKZ6014WW3HTTFJFQBF7359");
        var userEntities = await userRepository.GetListAsync();
        if (!userEntities.Any()) return "Invalid username or password";
        var user = userEntities.SingleOrDefault();
        if (user is null)
            return "Invalid username or password";
        var token = service.GenerateToken(user);
        user.Password = string.Empty;
        return token;
    };
}

Func<IRepository<UserEntity>, TokenService, UserEntity, Task<string>> SaveUser()
{
    return async (repository, service, entity) =>
    {
        var user = await repository.GetListAsync(x => x.Name == entity.Name);
        if (user.Count > 0)
            throw new Exception();
        entity.Password = service.GetPasswordHash(entity.Password);
        return await repository.InsertAsync(entity);
    };
}