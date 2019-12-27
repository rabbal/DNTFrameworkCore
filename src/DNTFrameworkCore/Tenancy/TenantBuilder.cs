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
    public sealed class TenantBuilder
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
        
//        /// <summary>
//        /// Register tenant specific options
//        /// </summary>
//        /// <typeparam name="TOptions">Type of options we are apply configuration to</typeparam>
//        /// <returns></returns>
//        public TenantBuilder WithPerTenantOptions<TOptions>(Action<TOptions, Tenant> setup)
//            where TOptions : class, new()
//        {
//            //Register the multi-tenant cache
//            Services.AddSingleton<IOptionsMonitorCache<TOptions>>(a =>
//                ActivatorUtilities.CreateInstance<TenantOptionsCache<TOptions>>(a));
//
//            //Register the multi-tenant options factory
//            Services.AddTransient<IOptionsFactory<TOptions>>(a =>
//                ActivatorUtilities.CreateInstance<TenantOptionsFactory<TOptions>>(a, setup));
//
//            //Register IOptionsSnapshot support
//            Services.AddScoped<IOptionsSnapshot<TOptions>>(a =>
//                ActivatorUtilities.CreateInstance<TenantOptions<TOptions>>(a));
//
//            //Register IOptions support
//            Services.AddSingleton<IOptions<TOptions>>(
//                a => ActivatorUtilities.CreateInstance<TenantOptions<TOptions>>(a));
//
//            return this;
//        }
    }
}