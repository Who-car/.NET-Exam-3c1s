using System.Reflection;
using System.Text;
using AutoMapper;
using Backend.Application.Commands.Options;
using Backend.Application.Dispatchers;
using Backend.Domain.Abstractions.Commands;
using Backend.Domain.Abstractions.Queries;
using Backend.WebAPI.Consumers;
using Backend.WebAPI.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Backend.WebAPI.Common.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMiddlewares(this IServiceCollection services)
    {
        services.AddScoped<ExceptionMiddleware>();

        return services;
    }
    
    public static IServiceCollection AddConfiguredAutoMapper(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg =>
        {
            cfg.AddProfiles( new List<Profile>
                {
                }
            );
        });
        
        return services;
    }
    
    public static IServiceCollection AddConfiguredAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.IncludeErrorDetails = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Authentication:JWT:Secret"]!)
                    ),
                    ValidateIssuerSigningKey = true,
                };
                // for SignalR
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = async context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        if (!string.IsNullOrEmpty(accessToken))
                            context.Token = accessToken;
                    }
                };
            });

        return services;
    }
    
    public static IServiceCollection AddConfiguredSwaggerGen(this IServiceCollection services)
    {
        services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
    
    public static IServiceCollection AddConfiguredMassTransit(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        services.AddMassTransit(busConfigurator =>
        {
            busConfigurator.SetKebabCaseEndpointNameFormatter();

            busConfigurator.AddConsumer<MovesConsumer>();
            
            busConfigurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.UseRawJsonDeserializer();
                
                cfg.Host(configuration["RabbitMq:Host"], hc =>
                {
                    hc.Username(configuration["RabbitMq:Username"] ?? "guest");
                    hc.Password(configuration["RabbitMq:Password"] ?? "guest");
                });
            });
        });
        
        return services;
    }
    
    public static IServiceCollection ConfigureCommandHandlers(this IServiceCollection services)
    {
        services.AddScoped<ICommandDispatcher, CommandDispatcher>();
        
        var assembly = Assembly.Load("Backend.Application");
        var commandTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { HandlerType = t, InterfaceType = i })
            .Where(x => x.InterfaceType.IsGenericType && 
                        (x.InterfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<>) || 
                         x.InterfaceType.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)))
            .ToList();

        foreach (var handler in commandTypes)
            services.AddScoped(handler.InterfaceType, handler.HandlerType);
        
        return services;
    }
    
    public static IServiceCollection ConfigureQueryHandlers(this IServiceCollection services)
    {
        services.AddScoped<IQueryDispatcher, QueryDispatcher>();
        
        var assembly = Assembly.Load("Backend.Data");
        var commandTypes = assembly.GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface)
            .SelectMany(t => t.GetInterfaces(), (t, i) => new { HandlerType = t, InterfaceType = i })
            .Where(x => x.InterfaceType.IsGenericType && 
                        x.InterfaceType.GetGenericTypeDefinition() == typeof(IQueryHandler<,>))
            .ToList();

        foreach (var handler in commandTypes)
            services.AddScoped(handler.InterfaceType, handler.HandlerType);
        
        return services;
    }
    
    public static IServiceCollection ConfigureServices(
        this IServiceCollection services, 
        IHostEnvironment environment,
        IConfiguration configuration)
    {
        services.Configure<AuthenticationOptions>(configuration.GetSection("Authentication:JWT"));

        return services;
    }
}