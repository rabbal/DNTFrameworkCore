using System.Linq;
using DNTFrameworkCore.TestCqrsAPI.Domain.Orders;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace DNTFrameworkCore.TestCqrsAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
