using DNTFrameworkCore.TestTenancy.Domain.Tasks;
using DNTFrameworkCore.TestTenancy.Infrastructure.Context;

namespace DNTFrameworkCore.TestTenancy.Infrastructure
{
    public static class InfrastructureRegistry
    {
        private static ILoggerFactory BuildLoggerFactory()
        {
            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging(builder =>
                builder.AddConsole()
                    //.AddFilter(category: DbLoggerCategory.Database.Command.Name, level: LogLevel.Information));
                    .AddFilter(level => true)); // log everything
            return serviceCollection.BuildServiceProvider().GetRequiredService<ILoggerFactory>();
        }

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddEFCore<ProjectDbContext>()
                .WithTrackingHook<long>()
                //.WithTenancyHook<long>() need to AddTenancy
                .WithDeletedEntityHook()
                .WithRowLevelSecurityHook<long>()
                .WithNumberingHook(options =>
                {
                    options.NumberedEntityMap[typeof(Task)] = new NumberedEntityOption
                    {
                        Prefix = "Task",
                        FieldNames = new[] {nameof(Task.BranchId)}
                    };
                });

            services.AddDbContext<ProjectDbContext>(builder =>
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
                        warnings.Throw(RelationalEventId.QueryClientEvaluationWarning);
                        warnings.Throw(CoreEventId.IncludeIgnoredWarning);
                    });
                //.UseLoggerFactory(BuildLoggerFactory());
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