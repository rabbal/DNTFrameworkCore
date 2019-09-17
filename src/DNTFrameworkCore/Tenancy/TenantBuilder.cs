using System;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Tenancy
{
    /// <summary>
    /// Nice method to create the tenant builder
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add the services (default tenant class)
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static TenantBuilder AddTenancy(this IServiceCollection services)
            => new TenantBuilder(services);
    }

    /// <summary>
    /// Configure tenant services
    /// </summary>
    public class TenantBuilder
    {
        public IServiceCollection Services { get; }

        public TenantBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Register the tenant resolver implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder WithResolutionStrategy<T>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where T : class, ITenantResolutionStrategy
        {
            Services.Add(ServiceDescriptor.Describe(typeof(ITenantResolutionStrategy), typeof(T), lifetime));
            return this;
        }


        /// <summary>
        /// Register the tenant store implementation
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lifetime"></param>
        /// <returns></returns>
        public TenantBuilder WithStore<T>(ServiceLifetime lifetime = ServiceLifetime.Transient)
            where T : class, ITenantStore
        {
            Services.Add(ServiceDescriptor.Describe(typeof(ITenantStore), typeof(T), lifetime));
            return this;
        }

        /// <summary>
        /// Register the tenancy options
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public TenantBuilder WithOptions(Action<TenancyOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            Services.Configure(options);

            return this;
        }
    }
}