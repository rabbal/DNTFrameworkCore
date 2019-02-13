using System;
using System.Threading.Tasks;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using StructureMap;

namespace DNTFrameworkCore.Web.MultiTenancy.Internal
{
    internal class StructureMapTenantContainerBuilder : ITenantContainerBuilder
    {
        public StructureMapTenantContainerBuilder(IContainer container,
            Action<TenantInfo, ConfigurationExpression> configure)
        {
            Guard.ArgumentNotNull(container, nameof(container));
            Guard.ArgumentNotNull(configure, nameof(configure));

            _container = container;
            _configure = configure;
        }

        private readonly IContainer _container;
        private readonly Action<TenantInfo, ConfigurationExpression> _configure;

        public Task<IContainer> BuildAsync(TenantInfo tenant)
        {
            Guard.ArgumentNotNull(tenant, nameof(tenant));

            var tenantContainer = _container.CreateChildContainer();
            tenantContainer.Configure(config => _configure(tenant, config));

            return Task.FromResult(tenantContainer);
        }
    }
}
