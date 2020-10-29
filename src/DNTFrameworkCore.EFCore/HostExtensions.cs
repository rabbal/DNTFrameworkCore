using System;
using DNTFrameworkCore.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Polly;

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
                var setup = provider.GetService<IDbSetup>();
                var context = provider.GetRequiredService<TContext>();

                try
                {
                    logger.LogInformation($"Migrating database associated with context {typeof(TContext).Name}");

                    var retry = Policy.Handle<Exception>().WaitAndRetry(new[]
                        {
                            TimeSpan.FromSeconds(5),
                            TimeSpan.FromSeconds(10),
                            TimeSpan.FromSeconds(15),
                        },
                        (exception, timeSpan, retry, ctx) =>
                        {
                            logger.LogWarning(exception,
                                "[{prefix}] Exception {ExceptionType} with message {Message} detected on attempt {retry} of {retries}",
                                nameof(TContext), exception.GetType().Name, exception.Message, retry, 3);
                        });

                    retry.Execute(() =>
                    {
                        context.Database.Migrate();
                        setup?.Seed();
                    });

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