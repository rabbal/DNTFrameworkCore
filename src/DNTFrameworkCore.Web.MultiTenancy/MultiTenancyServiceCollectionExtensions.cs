using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    public static class MultiTenancyServiceCollectionExtensions
    {
        public static IServiceCollection AddDNTMultiTenancy<TResolver>(this IServiceCollection services,
            MultiTenancyDatabaseStrategy strategy = MultiTenancyDatabaseStrategy.HybridDatabase)
            where TResolver : class, ITenantResolver
        {
            Guard.ArgumentNotNull(services, nameof(services));

            services.TryAddSingleton(
                Options.Create(new MultiTenancyOptions { Enabled = true, DatabaseStrategy = strategy }));

            services.AddScoped<ITenantResolver, TResolver>();

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Make Tenant and TenantContext injectable
            services.AddScoped(prov => prov.GetService<IHttpContextAccessor>()?.HttpContext?.GetTenantContext());
            services.AddScoped(prov => prov.GetService<TenantContext>()?.Tenant);

            // Make ITenant injectable for handling null injection, similar to IOptions
            services.Replace(ServiceDescriptor.Scoped(typeof(ITenant), prov => new TenantWrapper(prov.GetService<TenantInfo>())));

            // Ensure caching is available for caching resolvers
            //todo:Implement MemoryCacheTenantResolve
            //            var resolverType = typeof(TResolver);
            //            if (typeof(MemoryCacheTenantResolver).IsAssignableFrom(resolverType))
            //            {
            //                services.AddMemoryCache();
            //            }

            return services;
        }
    }
}