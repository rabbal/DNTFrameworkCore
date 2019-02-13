using Microsoft.AspNetCore.Builder;

namespace DNTFrameworkCore.Web.Security.Throttling
{
    /// <summary>
    /// AntiDos Middleware Extensions
    /// </summary>
    public static class ThrottlingMiddlewareExtensions
    {
        /// <summary>
        /// Adds AntiDosMiddleware to the pipeline.
        /// </summary>
        public static IApplicationBuilder UseAntiDos(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ThrottlingMiddleware>();
        }
    }
}