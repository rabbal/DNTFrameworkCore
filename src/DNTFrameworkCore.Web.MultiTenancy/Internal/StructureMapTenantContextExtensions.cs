using System;
using DNTFrameworkCore.MultiTenancy;
using StructureMap;

namespace DNTFrameworkCore.Web.MultiTenancy.Internal
{
    internal static class StructureMapTenantContextExtensions
    {
        private const string TenantContainerKey = "TENANT_CONTAINER";

        public static IContainer GetTenantContainer(this TenantContext tenantContext)
        {
            if (tenantContext == null) throw new ArgumentNullException(nameof(tenantContext));

            if (tenantContext.Properties.TryGetValue(TenantContainerKey, out var tenantContainer))
            {
                return tenantContainer as IContainer;
            }

            return null;
        }

        public static void SetTenantContainer(this TenantContext tenantContext, IContainer tenantContainer)
        {
            if (tenantContext == null) throw new ArgumentNullException(nameof(tenantContext));
            if (tenantContainer == null) throw new ArgumentNullException(nameof(tenantContainer));

            tenantContext.Properties[TenantContainerKey] = tenantContainer;
        }
    }
}