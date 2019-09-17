using DNTFrameworkCore.Web.Tenancy.Internal;
using Microsoft.AspNetCore.Builder;

namespace DNTFrameworkCore.Web.Tenancy
{
    /// <summary>
    ///     Nice method to register our middleware
    /// </summary>
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        ///     Use the Tenant Middleware to process the request
        /// </summary>
        public static IApplicationBuilder UseTenancy(this IApplicationBuilder builder) =>
            builder.UseMiddleware<TenantMiddleware>();

        /// <summary>
        ///     Use the Tenant Container Middleware to TenantScoped Dependency Injection
        /// </summary>
        public static IApplicationBuilder UseTenantContainer(this IApplicationBuilder builder) =>
            builder.UseMiddleware<TenantContainerMiddleware>();
    }
}