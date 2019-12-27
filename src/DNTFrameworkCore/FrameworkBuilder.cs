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
        public static FrameworkBuilder AddFramework(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped<IEventBus, EventBus>();
            services.AddSingleton<IDateTime, SystemDateTime>();
            services.AddTransient(typeof(Lazy<>), typeof(LazyFactory<>));

            return new FrameworkBuilder(services);
        }
    }

    /// <summary>
    /// Configure DNTFrameworkCore services
    /// </summary>
    public sealed class FrameworkBuilder
    {
        public IServiceCollection Services { get; }

        public FrameworkBuilder(IServiceCollection services)
        {
            Services = services;
        }

        /// <summary>
        /// Register the ISecurityService
        /// </summary>
        public FrameworkBuilder WithSecurityService()
        {
            Services.AddSingleton<ISecurityService, SecurityService>();
            return this;
        }
        
        /// <summary>
        /// Register the ICacheService
        /// </summary>
        public FrameworkBuilder WithMemoryCache()
        {
            Services.AddMemoryCache();
            Services.AddSingleton<ICacheService, MemoryCacheService>();
            return this;
        }
        
        /// <summary>
        /// Register the IBackgroundTaskQueue
        /// </summary>
        public FrameworkBuilder WithBackgroundTaskQueue()
        {
            Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            return this;
        }

        /// <summary>
        /// Register the IRandomNumberProvider
        /// </summary>
        public FrameworkBuilder WithRandomNumber()
        {
            Services.AddSingleton<IRandomNumber, RandomNumber>();
            return this;
        }
        
        /// <summary>
        /// Register the validation infrastructure's services
        /// </summary>
        public FrameworkBuilder WithModelValidation(Action<ValidationOptions> setupAction = null)
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