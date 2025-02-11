using Backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Backend.WebAPI.Common.Extensions;

public static class WebApplicationExtensions
{
    public static async Task AddMigrations(this WebApplication application)
    {
        using var scope = application.Services.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
        if (pendingMigrations.Any())
            await context.Database.MigrateAsync();
    }
}