using System;
using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.EFCore.SqlServer;
using EFCoreSecondLevelCacheInterceptor;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Infrastructure.Context;

namespace ProjectName.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEFCore<ProjectNameDbContext>()
                .WithTrackingHook<long>()
                .WithDeletedEntityHook()
                .WithRowLevelSecurityHook<long>()
                .WithRowIntegrityHook()
                .WithNumberingHook(options =>
                {
                    // options[typeof(Task)] = new[]
                    // {
                    //     new NumberedEntityOption
                    //     {
                    //         Prefix = "Task",
                    //         Fields = new[] {nameof(Task.BranchId)}
                    //     }
                    // };
                });

            services.AddDbContext<ProjectNameDbContext>((provider, builder) =>
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

            //TODO: See https://github.com/VahidN/EFCoreSecondLevelCacheInterceptor
            services.AddEFSecondLevelCache(options =>
                options.UseMemoryCacheProvider(CacheExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .DisableLogging(true));
        }
    }
}