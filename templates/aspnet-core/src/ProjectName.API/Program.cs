using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.EFCore.Logging;
using DNTFrameworkCore.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectName.Infrastructure.Context;

namespace ProjectName.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                .MigrateDbContext<ProjectNameDbContext>()
                .Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>().UseIISIntegration())
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddFile();

                    if (hostingContext.HostingEnvironment.IsProduction())
                    {
                        logging.AddEFCore<ProjectNameDbContext>();
                    }

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                        logging.AddEventLog();
                    }
                });
    }
}