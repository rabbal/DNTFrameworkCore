using System;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Common.Localization;
using ProjectName.Infrastructure.Context;

namespace ProjectName.UnitTests
{
    public enum DatabaseEngine
    {
        InMemory,
        SQLite
    }

    public static class TestingHelper
    {
        public static IServiceProvider PrepareServices(Action<IServiceCollection> configure = null,
            DatabaseEngine database = DatabaseEngine.InMemory, SqliteConnection connection = null)
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services.AddScoped(_ => NullTranslationService.Instance);
            services.AddEFCore<ProjectNameDbContext>();

            switch (database)
            {
                case DatabaseEngine.SQLite:
                    services.AddEntityFrameworkSqlite()
                        .AddDbContext<ProjectNameDbContext>(builder =>
                            builder.UseSqlite(connection ?? throw new ArgumentNullException(nameof(connection))));
                    break;
                case DatabaseEngine.InMemory:
                    services.AddEntityFrameworkInMemoryDatabase()
                        .AddDbContext<ProjectNameDbContext>(
                            builder => builder.UseInMemoryDatabase("SharedDatabaseName"));
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