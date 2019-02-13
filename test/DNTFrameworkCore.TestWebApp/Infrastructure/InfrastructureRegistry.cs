using DNTFrameworkCore.EntityFramework;
using DNTFrameworkCore.TestWebApp.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.TestWebApp.Infrastructure
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

        public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration
        )
        {
            services.AddDNTUnitOfWork<ProjectDbContext>();
            services.AddDbContext<ProjectDbContext>(builder =>
            {
                builder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        optionsBuilder =>
                        {
//                            var minutes = (int) TimeSpan
//                                .FromMinutes(Configuration.GetValue(nameof(optionsBuilder.CommandTimeout),
//                                    defaultValue: 3)).TotalSeconds;
//                            optionsBuilder.CommandTimeout(minutes);
                            // optionsBuilder.UseRowNumberForPaging();
                        })
                    .ConfigureWarnings(warnings =>
                    {
                        warnings.Throw(RelationalEventId.QueryClientEvaluationWarning);
                        warnings.Throw(CoreEventId.IncludeIgnoredWarning);
                    })
                    .UseLoggerFactory(BuildLoggerFactory());
            });
        }
    }
}