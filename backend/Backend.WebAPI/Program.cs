using Backend.Data;
using Backend.WebAPI.Common.Extensions;
using Backend.WebAPI.Hubs;
using Backend.WebAPI.Middlewares;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);

builder.Services.AddDbContext<PostgresDbContext>(options =>
    options.UseLazyLoadingProxies()
        .UseNpgsql(builder.Configuration["Database:PostgreSQL:ConnectionString"])
        .UseSnakeCaseNamingConvention());

builder.Services.AddSingleton(new MongoDbContext(builder.Configuration.GetSection("Database:MongoDB")));

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

var application = builder.Build();

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
application.MapHub<RoomHub>("api/hubs/room");

application.Run();