using System.Threading.Tasks;
using DNTFrameworkCore.Tenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Tenancy.Internal
{
    internal sealed class TenantResolutionMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolutionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (!context.Items.ContainsKey(TenancyConstants.HttpContextItemName))
            {
                var strategy = context.RequestServices.GetRequiredService<ITenantResolutionStrategy>();
                var store = context.RequestServices.GetRequiredService<ITenantStore>();

                var tenantId = strategy.ResolveTenantId();
                var tenant = await store.FindTenantAsync(tenantId);

                context.Items.Add(TenancyConstants.HttpContextItemName, tenant);
            }

            await _next.Invoke(context);
        }
    }
}