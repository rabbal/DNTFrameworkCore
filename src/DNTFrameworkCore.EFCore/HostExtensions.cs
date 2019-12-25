using System;
using DNTFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.EFCore
{
    public static class HostExtensions
    {
        public static IHost MigrateDbContext<TContext>(this IHost host) where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                var provider = scope.ServiceProvider;

                var logger = provider.GetRequiredService<ILogger<TContext>>();
                var db = provider.GetService<IDbSetup>();
                var context = provider.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    context.Database.Migrate();

                    db?.Seed();

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return host;
        }
    }
}