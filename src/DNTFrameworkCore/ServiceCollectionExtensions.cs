using System;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Caching;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Threading.BackgroundTasks;
using DNTFrameworkCore.Timing;
using DNTFrameworkCore.Validation;
using DNTFrameworkCore.Validation.Interception;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static IServiceCollection AddDNTFrameworkCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddScoped<ITenant>(provider => new TenantWrapper(null));
            services.AddTransient<PermissionDependencyContext>();
            services.AddScoped<IEventBus, EventBus>();
            services.AddScoped<IUserSession>(provider => NullUserSession.Instance);
            services.AddSingleton<IRandomNumberProvider, RandomNumberProvider>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddSingleton<IPermissionService, PermissionService>();
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddScoped<IDateTime, Timing.DateTime>();
            services.AddTransient(typeof(Lazy<>), typeof(LazyFactory<>));

            return services;
        }

        public static IServiceCollection AddModelValidation(
            this IServiceCollection services,
            Action<ValidationOptions> setupAction = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddTransient<ValidationInterceptor>();
            services.AddTransient<MethodInvocationValidator>();
            services.AddTransient<IMethodParameterValidator, DataAnnotationMethodParameterValidator>();
            services.AddTransient<IMethodParameterValidator, ValidatableObjectMethodParameterValidator>();
            services.AddTransient<IMethodParameterValidator, ModelValidationMethodParameterValidator>();

            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            return services;
        }
    }
}