using System;
using DNTFrameworkCore.EFCore;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Infrastructure.Context;
using ProjectName.Infrastructure.Interception;

namespace ProjectName.Infrastructure
{
    public static class InfrastructureRegistry
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEFCore<ProjectNameDbContext>()
                .WithTrackingHook<long>()
                .WithDeletedEntityHook()
                .WithRowIntegrityHook();

            services.AddDbContext<ProjectNameDbContext>((provider, builder) =>
            {
                builder.AddInterceptors(provider.GetRequiredService<SecondLevelCacheInterceptor>());
                builder.AddInterceptors(new PersianYeKeInterceptor());
                
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
                options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .DisableLogging(true));
        }
    }
}