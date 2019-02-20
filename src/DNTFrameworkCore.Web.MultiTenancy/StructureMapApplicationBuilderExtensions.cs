using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Web.MultiTenancy.Internal;
using Microsoft.AspNetCore.Builder;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    public static class StructureMapApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseDNTTenantContainers(this IApplicationBuilder app)
        {
            Guard.ArgumentNotNull(app, nameof(app));
           
            return app.UseMiddleware<MultiTenantContainerMiddleware>();
        }
    }
}
