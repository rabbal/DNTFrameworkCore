using System;
using DNTFrameworkCore.Auditing;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Caching;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Threading.BackgroundTasks;
using DNTFrameworkCore.Timing;
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
            services.AddMemoryCache();
            services.AddSingleton<ICacheService, MemoryCacheService>();
            services.AddScoped<IDateTime, Timing.DateTime>();

            return new DNTBuilder(services);
        }

        public static IDNTBuilder AddAuditing(
            this IDNTBuilder builder,
            Action<AuditingOptions> setupAction = null)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.Services.AddScoped<AuditingInterceptor>();
            builder.Services.AddScoped<IAuditingHelper, AuditingHelper>();
            builder.Services.AddScoped<IAuditingStore, DefaultAuditingStore>();

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            return builder;
        }

        public static IDNTBuilder AddModelValidation(
            this IDNTBuilder builder,
            Action<ValidationOptions> setupAction = null)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.Services.AddTransient<ValidationInterceptor>();
            builder.Services.AddTransient<MethodInvocationValidator>();
            builder.Services.AddTransient<IMethodParameterValidator, DataAnnotationMethodParameterValidator>();
            builder.Services.AddTransient<IMethodParameterValidator, ValidatableObjectMethodParameterValidator>();
            builder.Services.AddTransient<IMethodParameterValidator, ModelValidationMethodParameterValidator>();

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            return builder;
        }

        public static IDNTBuilder AddTransaction(
            this IDNTBuilder builder,
            Action<TransactionOptions> setupAction = null)
        {
            Guard.ArgumentNotNull(builder, nameof(builder));

            builder.Services.AddTransient<TransactionInterceptor>();
            builder.Services.AddScoped<ITransactionProvider>(provider => NullTransactionProvider.Instance);

            if (setupAction != null)
            {
                builder.Services.Configure(setupAction);
            }

            return builder;
        }
    }
}