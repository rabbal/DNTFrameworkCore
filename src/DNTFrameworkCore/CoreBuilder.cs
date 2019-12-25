using System;
using DNTFrameworkCore.Caching;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Eventing;
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
        public static CoreBuilder AddDNTFrameworkCore(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IEventBus, EventBus>();
            services.AddSingleton<IDateTime, SystemDateTime>();
            services.AddTransient(typeof(Lazy<>), typeof(LazyFactory<>));

            return new CoreBuilder(services);
        }
    }

    /// <summary>
    /// Configure DNTFrameworkCore services
    /// </summary>
    public sealed class CoreBuilder
    {
        public IServiceCollection Services { get; }

        public CoreBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Register the ISecurityService
        /// </summary>
        public CoreBuilder WithSecurityService()
        {
            Services.AddSingleton<ISecurityService, SecurityService>();
            return this;
        }
        
        /// <summary>
        /// Register the ICacheService
        /// </summary>
        public CoreBuilder WithMemoryCache()
        {
            Services.AddMemoryCache();
            Services.AddSingleton<ICacheService, MemoryCacheService>();
            return this;
        }
        
        /// <summary>
        /// Register the IBackgroundTaskQueue
        /// </summary>
        public CoreBuilder WithBackgroundTaskQueue()
        {
            Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            return this;
        }

        /// <summary>
        /// Register the IRandomNumberProvider
        /// </summary>
        public CoreBuilder WithRandomNumberProvider()
        {
            Services.AddSingleton<IRandomNumberProvider, RandomNumberProvider>();
            return this;
        }
        
        /// <summary>
        /// Register the validation infrastructure's services
        /// </summary>
        public CoreBuilder WithModelValidation(Action<ValidationOptions> setupAction = null)
        {
            Services.AddTransient<ValidationInterceptor>();
            Services.AddTransient<MethodInvocationValidator>();
            Services.AddTransient<IMethodParameterValidator, DataAnnotationMethodParameterValidator>();
            Services.AddTransient<IMethodParameterValidator, ValidatableObjectMethodParameterValidator>();
            Services.AddTransient<IMethodParameterValidator, ModelValidationMethodParameterValidator>();

            if (setupAction != null)
            {
                Services.Configure(setupAction);
            }

            return this;
        }
    }
}