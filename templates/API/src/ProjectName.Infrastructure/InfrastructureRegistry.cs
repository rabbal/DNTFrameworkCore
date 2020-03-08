using System;
using System.Threading.Tasks;
using CacheManager.Core;
using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.SqlServer;
using DNTFrameworkCore.Numbering;
using EFSecondLevelCache.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ProjectName.Infrastructure.Context;
using ProjectName.Infrastructure.Hooks;

namespace ProjectName.Infrastructure
{
    public static class InfrastructureRegistry
    {
        private static ILoggerFactory CreateLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                builder.AddConsole()
                    .AddFilter(category: DbLoggerCategory.Database.Command.Name, level: LogLevel.Information));
                    //.AddFilter(level => true)); // log everything
            return serviceCollection.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        }
        
        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration
        )
        {
            services.AddEFCore<ProjectNameDbContext>()
                .WithTrackingHook<long>()
                .WithDeletedEntityHook()
                .WithRowIntegrityHook();
            
            services.AddTransient<IHook, PreInsertApplyCorrectYeKeHook>();
            services.AddTransient<IHook, PreUpdateApplyCorrectYeKeHook>();
            
            services.AddDbContext<ProjectNameDbContext>(builder =>
            {
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
                    //Todo:In Development CreateLoggerFactory(BuildLoggerFactory());
            });
            
            services.AddEFSecondLevelCache();

            // Add an in-memory cache service provider
            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration),
                new CacheManager.Core.ConfigurationBuilder()
                    .WithJsonSerializer()
                    .WithMicrosoftMemoryCacheHandle("MemoryCache1")
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .Build());
        }
    }
}