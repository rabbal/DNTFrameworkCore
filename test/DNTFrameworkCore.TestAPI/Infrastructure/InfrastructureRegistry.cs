using System;
using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.EFCore.SqlServer;
using DNTFrameworkCore.Numbering;
using DNTFrameworkCore.TestAPI.Domain.Tasks;
using DNTFrameworkCore.TestAPI.Infrastructure.Context;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestAPI.Infrastructure
{
    public static class InfrastructureRegistry
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEFCore<ProjectDbContext>()
                .WithTrackingHook<long>()
                .WithDeletedEntityHook()
                .WithRowLevelSecurityHook<long>()
                .WithRowIntegrityHook()
                .WithNumberingHook(options =>
                {
                    options[typeof(Task)] = new[]
                    {
                        new NumberedEntityOption
                        {
                            Prefix = "Task",
                            Fields = new[] {nameof(Task.BranchId)}
                        }
                    };
                });

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
                        })
                    .ConfigureWarnings(warnings =>
                    {
                        //...
                    });
            });

            services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .DisableLogging(true));
        }
    }
}