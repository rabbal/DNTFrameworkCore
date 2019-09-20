using System;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Tenancy
{
    public interface ITenantContainerFactory : ISingletonDependency
    {
        IServiceProvider CreateContainer(string tenantId);
    }
}