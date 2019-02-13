using System;
using System.Threading.Tasks;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using Microsoft.AspNetCore.Http;
using StructureMap;

namespace DNTFrameworkCore.Web.MultiTenancy.Internal
{
    internal class MultiTenantContainerMiddleware
    {
        private readonly RequestDelegate _next;

        public MultiTenantContainerMiddleware(RequestDelegate next)
        {
            Guard.ArgumentNotNull(next, nameof(next));
            
            _next = next;
        }

        public async Task Invoke(HttpContext context, Lazy<ITenantContainerBuilder> builder)
        {
            Guard.ArgumentNotNull(context, nameof(context));

            var tenantContext = context.GetTenantContext();

            if (tenantContext != null)
            {
                var tenantContainer = await GetTenantContainerAsync(tenantContext, builder);

                using (var requestContainer = tenantContainer.GetNestedContainer())
                {
                    // Replace the request IServiceProvider created by IServiceScopeFactory
                    context.RequestServices = requestContainer.GetInstance<IServiceProvider>();
                    await _next.Invoke(context);
                }
            }
        }

        private static async Task<IContainer> GetTenantContainerAsync(
            TenantContext tenantContext, 
            Lazy<ITenantContainerBuilder> builder)
        {
            var tenantContainer = tenantContext.GetTenantContainer();

            if (tenantContainer != null) return tenantContainer;
            
            tenantContainer = await builder.Value.BuildAsync(tenantContext.Tenant);
            
            tenantContext.SetTenantContainer(tenantContainer);

            return tenantContainer;
        }
    }
}