using System;
using System.IO;
using System.Reflection;
using DNTFrameworkCore;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.Web;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ProjectName.Application;
using ProjectName.Application.Localization;
using ProjectName.Infrastructure.Context;
using ProjectName.IntegrationTests.Stubs;

namespace ProjectName.IntegrationTests
{
    public enum DatabaseEngine
    {
        LocalDb,
        SqlServerExpress,
        SQLite,
    }

    public static class TestingHelper
    {
        public static IServiceProvider BuildServiceProvider(DatabaseEngine database, SqliteConnection connection = null,
            Action<IServiceCollection> configure = null)
        {
            var services = new ServiceCollection();

            services.AddApplication();
            services.AddLogging();
            services.AddScoped(_ => NullTranslationService.Instance);
            services.AddEFCore<ProjectNameDbContext>();
            services.AddFramework()
                .WithModelValidation()
                .WithFluentValidation();
            services.AddWebFramework()
                .WithPasswordHashAlgorithm();

            var fileName = Path.Combine(Path.GetDirectoryName(typeof(TestingHelper).GetTypeInfo().Assembly.Location),
                "ProjectIntegrationTesting.mdf");
            switch (database)
            {
                case DatabaseEngine.LocalDb:
                    services.AddEntityFrameworkSqlServer()
                        .AddDbContext<ProjectNameDbContext>(builder =>
                            builder.UseSqlServer(
                                $@"Data Source=(LocalDB)\MSSQLLocalDb;Initial Catalog=ProjectNameIntegrationTesting;Integrated Security=True;
                                    MultipleActiveResultSets=true;AttachDbFileName={fileName}"));
                    break;
                case DatabaseEngine.SQLite:
                    services.AddEntityFrameworkSqlite()
                        .AddDbContext<ProjectNameDbContext>(builder =>
                            builder.UseSqlite(connection ?? throw new ArgumentNullException(nameof(connection))));
                    break;
                case DatabaseEngine.SqlServerExpress:
                    services.AddEntityFrameworkSqlServer()
                        .AddDbContext<ProjectNameDbContext>(builder =>
                            builder.UseSqlServer(
                                $@"Data Source=.\SQLEXPRESS;Initial Catalog=ProjectNameIntegrationTesting;Integrated Security=True;
                                    MultipleActiveResultSets=true;AttachDbFileName={fileName};User Instance=True"));


                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(database), database, null);
            }

            //Exclude business events to simplify test-data management 
            services.Replace(ServiceDescriptor.Singleton<IEventBus, StubEventBus>());

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