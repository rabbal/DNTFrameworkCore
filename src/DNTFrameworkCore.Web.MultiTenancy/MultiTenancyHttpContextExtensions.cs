using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    /// <summary>
    /// Multitenant extensions for <see cref="HttpContext"/>.
    /// </summary>
    public static class MultiTenancyHttpContextExtensions
    {
        private const string TenantContextKey = "DNTFramework.MultiTenancy.TenantContext";

        public static void SetTenantContext(this HttpContext context, TenantContext tenantContext)
        {
            Guard.ArgumentNotNull(context, nameof(context));
            Guard.ArgumentNotNull(tenantContext, nameof(tenantContext));

            context.Items[TenantContextKey] = tenantContext;
        }

        public static TenantContext GetTenantContext(this HttpContext context)
        {
            Guard.ArgumentNotNull(context, nameof(context));

            if (context.Items.TryGetValue(TenantContextKey, out var tenantContext))
            {
                return tenantContext as TenantContext;
            }

            return null;
        }

        public static TenantInfo GetTenant(this HttpContext context)
        {
            Guard.ArgumentNotNull(context, nameof(context));

            var tenantContext = GetTenantContext(context);

            return tenantContext?.Tenant;
        }
    }
}