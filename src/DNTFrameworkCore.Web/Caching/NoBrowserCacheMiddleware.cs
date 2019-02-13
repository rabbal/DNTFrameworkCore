using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Caching
{
    public class NoBrowserCacheMiddleware
    {
        private readonly RequestDelegate _next;

        public NoBrowserCacheMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            context.DisableBrowserCache();
            return _next(context);
        }
    }

    public static class NoBrowserCacheMiddlewareExtensions
    {
        public static IApplicationBuilder UseNoBrowserCache(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<NoBrowserCacheMiddleware>();
        }
    }
}