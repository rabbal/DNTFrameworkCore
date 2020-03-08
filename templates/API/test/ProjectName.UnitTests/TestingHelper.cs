using System;
using CacheManager.Core;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.Localization;
using EFSecondLevelCache.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Infrastructure.Context;
using ProjectName.Resources;

namespace ProjectName.UnitTests
{
    public enum DatabaseEngine
    {
        InMemory,
        SQLite
    }

    public static class TestingHelper
    {
        public static IServiceProvider BuildServiceProvider(Action<IServiceCollection> configure = null,
            DatabaseEngine database = DatabaseEngine.InMemory, SqliteConnection connection = null)
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddLocalization();
            services.AddNullLocalization();
            services.AddResources();
            services.AddEFCore<ProjectNameDbContext>();
            services.AddEFSecondLevelCache();
            services.AddSingleton(typeof(ICacheManager<>), typeof(BaseCacheManager<>));
            services.AddSingleton(typeof(ICacheManagerConfiguration),
                new ConfigurationBuilder()
                    .WithJsonSerializer()
                    .WithMicrosoftMemoryCacheHandle()
                    .WithExpiration(ExpirationMode.Absolute, TimeSpan.FromMinutes(10))
                    .Build());

            switch (database)
            {
                case DatabaseEngine.SQLite:
                    services.AddEntityFrameworkSqlite()
                        .AddDbContext<ProjectNameDbContext>(builder =>
                            builder.UseSqlite(connection ?? throw new ArgumentNullException(nameof(connection))));
                    break;
                case DatabaseEngine.InMemory:
                    services.AddEntityFrameworkInMemoryDatabase()
                        .AddDbContext<ProjectNameDbContext>(builder => builder.UseInMemoryDatabase("SharedDatabaseName"));
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(database), database, null);
            }


            configure?.Invoke(services);

            var provider = services.BuildServiceProvider();

            provider.RunScoped<ProjectNameDbContext>(context =>
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            });

            return provider;
        }
    }
}