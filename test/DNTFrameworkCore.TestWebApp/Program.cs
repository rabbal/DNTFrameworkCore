using DNTFrameworkCore.TestWebApp.Infrastructure.Context;
using DNTFrameworkCore.Web.EFCore;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DNTFrameworkCore.TestWebApp
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
                .UseStartup<Startup>();
    }
}