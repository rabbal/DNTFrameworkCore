using DNTFrameworkCore.EFCore;
using DNTFrameworkCore.TestWebApp.Infrastructure.Context;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(builder => builder.UseStartup<Startup>().UseIISIntegration());
    }
}