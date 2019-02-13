using System;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Web.MultiTenancy.Internal;
using Microsoft.AspNetCore.Builder;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    public static class MultiTenancyApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseMultiTenancy(this IApplicationBuilder app)
        {
            Guard.ArgumentNotNull(app, nameof(app));
            
            return app.UseMiddleware<TenantResolutionMiddleware>();
        }

        public static IApplicationBuilder UsePerTenant<TTenant>(this IApplicationBuilder app,
            Action<TenantPipelineBuilderContext, IApplicationBuilder> configuration)
        {
            Guard.ArgumentNotNull(app, nameof(app));
            Guard.ArgumentNotNull(configuration, nameof(configuration));

            app.Use(next => new TenantPipelineMiddleware(next, app, configuration).Invoke);

            return app;
        }
    }
}