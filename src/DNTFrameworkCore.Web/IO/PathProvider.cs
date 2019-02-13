using System;
using System.IO;
using DNTFrameworkCore.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.IO
{
    public static class PathProviderExtensions
    {
        public static IServiceCollection AddPathProvider(this IServiceCollection services)
        {
            services.AddSingleton<IPathProvider, PathProvider>();

            return services;
        }
    }

    internal class PathProvider : IPathProvider
    {
        private readonly IHostingEnvironment _hostingEnvironment;

        public PathProvider(IHostingEnvironment environment)
        {
            _hostingEnvironment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public string MapPath(string path)
        {
            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, path);
            return filePath;
        }
    }
}