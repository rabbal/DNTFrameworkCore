using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using StructureMap;

namespace DNTFrameworkCore.Web.MultiTenancy.Internal
{
    internal static class StructureMapTenantContextExtensions
    {
        private const string TenantContainerKey = "DNTFramework.MultiTenancy.TenantContainer";

        public static IContainer GetTenantContainer(this TenantContext tenantContext)
        {
            Guard.ArgumentNotNull(tenantContext, nameof(tenantContext));

            if (tenantContext.Properties.TryGetValue(TenantContainerKey, out var tenantContainer))
            {
                return tenantContainer as IContainer;
            }

            return null;
        }

        public static void SetTenantContainer(this TenantContext tenantContext, IContainer tenantContainer)
        {
            Guard.ArgumentNotNull(tenantContext, nameof(tenantContext));
            Guard.ArgumentNotNull(tenantContainer, nameof(tenantContainer));

            tenantContext.Properties[TenantContainerKey] = tenantContainer;
        }
    }
}