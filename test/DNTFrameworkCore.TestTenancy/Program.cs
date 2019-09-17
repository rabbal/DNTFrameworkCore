using DNTFrameworkCore.EFCore.Logging;
using DNTFrameworkCore.Logging;
using DNTFrameworkCore.TestTenancy.Infrastructure.Context;
using DNTFrameworkCore.Web.EFCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.TestTenancy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build()
                .MigrateDbContext<ProjectDbContext>()
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseDefaultServiceProvider((context, options) =>
                {
                    options.ValidateScopes = context.HostingEnvironment.IsDevelopment();
                })
                .ConfigureLogging((hostingContext, logging) =>
                {
                    logging.ClearProviders();
                    logging.AddConfiguration(hostingContext.Configuration.GetSection("Logging"));
                    logging.AddEFCore<ProjectDbContext>();
                    logging.AddFile();

                    if (hostingContext.HostingEnvironment.IsDevelopment())
                    {
                        logging.AddConsole();
                        logging.AddDebug();
                        logging.AddEventSourceLogger();
                        //logging.AddEventLog();
                    }
                })
                .UseUrls("http://localhost:7000", "https://localhost:7001", "http://localhost:7002")
                .UseStartup<Startup>();
    }
}