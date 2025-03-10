using Backend.Data;
using Backend.WebAPI.Common.Extensions;
using Backend.WebAPI.Hubs;
using Backend.WebAPI.Middlewares;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true)
    .AddEnvironmentVariables();

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration["Database:PostgreSQL:ConnectionString"])
        .UseSnakeCaseNamingConvention()
        .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning))
        );

builder.Services.AddSingleton(new MongoDbContext(builder.Configuration.GetSection("Database:MongoDB")));
builder.Services.AddSingleton<IMemoryCache, MemoryCache>();

builder.Services
    .AddRouting(opt => opt.LowercaseUrls = true)
    .ConfigureCommandHandlers()
    .ConfigureQueryHandlers()
    .AddMiddlewares()
    .AddAuthorization()
    .AddConfiguredAuthentication(builder.Configuration)
    .AddConfiguredAutoMapper()
    .AddConfiguredMassTransit(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddConfiguredSwaggerGen();

builder.Services.AddControllers();
builder.Services.AddSignalR();

builder.Services.AddCors(options => options.AddDefaultPolicy(policyBuilder =>
{
    var origins = builder.Configuration.GetSection("CorsPolicy:Origins").Get<string[]>()!;
    policyBuilder
        .WithOrigins(origins)
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials();
}));

builder.Services.ConfigureServices(builder.Environment, builder.Configuration);

var application = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await application.AddMigrations();

if (application.Environment.IsDevelopment())
{
    application
        .UseSwagger()
        .UseSwaggerUI();
}

application
    .UseCors()
    .UseMiddleware<ExceptionMiddleware>()
    .UseAuthentication()
    .UseAuthorization()
    .UseHttpsRedirection();

application.MapControllers();
application.MapHub<GameHub>("/game-session");

application.Run();