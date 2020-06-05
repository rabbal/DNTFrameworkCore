using System;
using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.TestWebApp.Infrastructure.Context;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestWebApp.Infrastructure
{
    public static class InfrastructureRegistry
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEFCore<ProjectDbContext>()
                .WithTrackingHook<long>();

            services.AddDbContext<ProjectDbContext>(builder =>
            {
                builder.AddInterceptors(new SecondLevelCacheInterceptor());
                builder.EnableSensitiveDataLogging();
                builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    optionsBuilder =>
                    {
                        var seconds = (int) TimeSpan
                            .FromMinutes(configuration.GetValue(nameof(optionsBuilder.CommandTimeout),
                                3)).TotalSeconds;
                        optionsBuilder.CommandTimeout(seconds);
                        optionsBuilder.MigrationsHistoryTable("MigrationHistory", "dbo");
                    });
            });

            services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(10)));
        }
    }
}