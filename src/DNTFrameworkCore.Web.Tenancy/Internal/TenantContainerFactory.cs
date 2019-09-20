using System;
using DNTFrameworkCore.Common;
using DNTFrameworkCore.Tenancy;
using DNTFrameworkCore.Web.Dependency;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Tenancy.Internal
{
    internal sealed class TenantContainerFactory : ITenantContainerFactory
    {
        private readonly LazyConcurrentDictionary<string, IServiceProvider> _providers =
            new LazyConcurrentDictionary<string, IServiceProvider>(StringComparer.OrdinalIgnoreCase);

        private readonly IServiceProvider _provider;
        private readonly IServiceCollection _services;

        public TenantContainerFactory(IServiceProvider provider, IServiceCollection services)
        {
            _provider = provider;
            _services = services;
        }

        public IServiceProvider CreateContainer(string tenantId)
        {
            return _providers.GetOrAdd(tenantId, key =>
            {
                var services = _provider.CreateChildContainer(_services);
                return services.BuildServiceProvider();
            });
        }
    }
}