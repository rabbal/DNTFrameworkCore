using System;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Web.MultiTenancy.Internal;
using StructureMap;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    public static class MultiTenancyContainerExtensions
    {
        public static void ConfigureTenants(this IContainer container, Action<ConfigurationExpression> configure)
        {
            Guard.ArgumentNotNull(container, nameof(container));
            Guard.ArgumentNotNull(configure, nameof(configure));

            container.Configure(_ =>
                _.For<ITenantContainerBuilder>()
                    .Use(new StructureMapTenantContainerBuilder(container, (tenant, config) => configure(config)))
            );
        }

        public static void ConfigureTenants(this IContainer container, Action<TenantInfo, ConfigurationExpression> configure)
        {
            Guard.ArgumentNotNull(container, nameof(container));
            Guard.ArgumentNotNull(configure, nameof(configure));

            container.Configure(_ =>
                _.For<ITenantContainerBuilder>()
                    .Use(new StructureMapTenantContainerBuilder(container, configure))
            );
        }
    }
}
