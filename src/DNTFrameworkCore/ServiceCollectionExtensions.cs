using System;
using DNTFrameworkCore.Auditing;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Caching;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Threading.BackgroundTasks;
using DNTFrameworkCore.Transaction;
using DNTFrameworkCore.Transaction.Interception;
using DNTFrameworkCore.Validation;
using DNTFrameworkCore.Validation.Interception;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IDNTBuilder AddDNTFramework(this IServiceCollection services)
        {
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddScoped<ITenant>(provider => new TenantWrapper(null));
            services.AddTransient<PermissionDependencyContext>();
            services.AddScoped<IEventBus, EventBus>();
            services.AddScoped<IUserSession>(provider => NullUserSession.Instance);
            services.AddSingleton<IRandomNumberProvider, RandomNumberProvider>();
            services.AddSingleton<ISecurityService, SecurityService>();
            services.AddSingleton<IPermissionService, PermissionService>();

            services.AddTransient<TransactionInterceptor>();
            services.AddScoped<ITransactionProvider>(provider => NullTransactionProvider.Instance);
            services.AddOptions<TransactionOptions>();

            services.AddTransient<ValidationInterceptor>();
            services.AddTransient<MethodInvocationValidator>();
            services.AddOptions<ValidationOptions>();

            services.AddScoped<AuditingInterceptor>();
            services.AddScoped<IAuditingHelper, AuditingHelper>();
            services.AddScoped<IAuditingStore, DefaultAuditingStore>();
            services.AddOptions<AuditingOptions>();

            return new DNTBuilder(services);
        }

        public static IDNTBuilder AddAuditingOptions(this IDNTBuilder builder,
            Action<AuditingOptions> setupAction)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));
            Guard.ArgumentNotNull(setupAction, nameof(setupAction));

            builder.Services.Configure(setupAction);
            return builder;
        }

        public static IDNTBuilder AddValidationOptions(this IDNTBuilder builder,
            Action<ValidationOptions> setupAction)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));
            Guard.ArgumentNotNull(setupAction, nameof(setupAction));

            builder.Services.Configure(setupAction);
            return builder;
        }

        public static IDNTBuilder AddDataAnnotationValidation(this IDNTBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.Services.AddTransient<IMethodParameterValidator, ValidatableObjectMethodParameterValidator>();
            builder.Services.AddTransient<IMethodParameterValidator, DataAnnotationMethodParameterValidator>();
            return builder;
        }

        public static IDNTBuilder AddModelValidation(this IDNTBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.Services.AddTransient<IMethodParameterValidator, ModelValidationMethodParameterValidator>();
            return builder;
        }

        public static IDNTBuilder AddTransactionOptions(this IDNTBuilder builder,
            Action<TransactionOptions> setupAction)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));
            Guard.ArgumentNotNull(setupAction, nameof(setupAction));

            builder.Services.Configure(setupAction);
            return builder;
        }

        public static IDNTBuilder AddMemoryCache(this IDNTBuilder builder)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<ICacheService, MemoryCacheService>();
            return builder;
        }
    }
}