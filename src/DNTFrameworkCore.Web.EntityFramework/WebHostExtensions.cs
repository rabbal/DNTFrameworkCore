using System;
using DNTFrameworkCore.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.EntityFramework
{
    public static class WebHostExtensions
    {
        public static IWebHost MigrateDbContext<TContext>(this IWebHost webHost) where TContext : DbContext
        {
            using (var scope = webHost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var logger = services.GetRequiredService<ILogger<TContext>>();
                var initializers = services.GetServices<IDbInitializer>();

                var context = services.GetService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    //if the sql server container is not created on run docker compose this
                    //migration can't fail for network related exception. The retry options for DbContext only 
                    //apply to transient exceptions.

                    context.Database.Migrate();

                    foreach (var initializer in initializers)
                    {
                        initializer.Initialize();
                    }

                    logger.LogInformation($"Migrated database associated with context {typeof(TContext).Name}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,
                        $"An error occurred while migrating the database used on context {typeof(TContext).Name}");
                }
            }

            return webHost;
        }
    }
}