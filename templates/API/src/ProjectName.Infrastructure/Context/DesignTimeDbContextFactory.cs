using System.IO;
using System.Linq;
using DNTFrameworkCore.EFCore.Context.Hooks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ProjectName.Infrastructure.Context
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ProjectNameDbContext>
    {
        public ProjectNameDbContext CreateDbContext(string[] args)
        {
            var services = new ServiceCollection();

            services.AddOptions();
            services.AddSingleton<ILoggerFactory, LoggerFactory>();
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var provider = services.BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<ProjectNameDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new ProjectNameDbContext(builder.Options, Enumerable.Empty<IHook>());
        }
    }
}