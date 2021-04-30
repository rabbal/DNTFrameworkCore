using System;
using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories;
using DNTFrameworkCore.TestCqrsAPI.Infrastructure.Context;
using DNTFrameworkCore.TestCqrsAPI.Infrastructure.Repositories.Catalog;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestCqrsAPI.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEFCore<ProjectDbContext>()
                .WithTrackingHook<long>()
                .WithDeletedEntityHook()
                .WithRowLevelSecurityHook<long>()
                .WithRowIntegrityHook();

            services.AddScoped<IPriceTypeRepository, PriceTypeRepository>();
            
            services.AddDbContext<ProjectDbContext>((provider, builder) =>
            {
                builder.AddInterceptors(provider.GetRequiredService<SecondLevelCacheInterceptor>());
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