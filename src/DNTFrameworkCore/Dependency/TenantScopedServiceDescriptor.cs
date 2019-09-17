using System;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Dependency
{
    /// <summary>
    /// Describes a tenant-scoped service
    /// </summary>
    public class TenantScopedServiceDescriptor : ServiceDescriptor
    {
        /// <summary>
        /// Initialises a new instance of <see cref="TenantScopedServiceDescriptor"/>
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="implementationType">The implementation type</param>
        public TenantScopedServiceDescriptor(Type serviceType, Type implementationType) 
            : base(serviceType, implementationType, ServiceLifetime.Singleton)
        {
        }

        /// <summary>
        /// Initialises a new instance of <see cref="TenantScopedServiceDescriptor"/>
        /// </summary>
        /// <param name="serviceType">The service type</param>
        /// <param name="factory">The service instance factory</param>
        public TenantScopedServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory) 
            : base(serviceType, factory, ServiceLifetime.Singleton)
        {
        }
    }
}