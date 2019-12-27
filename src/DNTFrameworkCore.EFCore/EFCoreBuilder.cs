using System;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.EFCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.Protection;
using DNTFrameworkCore.EFCore.Transaction;
using DNTFrameworkCore.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable InconsistentNaming

namespace DNTFrameworkCore.EFCore
{
    /// <summary>
    ///     Nice method to create the EFCore builder
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        ///     Add the services (application specific tenant class)
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static EFCoreBuilder AddEFCore<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext, IUnitOfWork
        {
            services.AddScoped(provider => (IUnitOfWork) provider.GetRequiredService(typeof(TDbContext)));
            services.AddTransient<TransactionInterceptor>();
            services.AddScoped<IKeyValueService, KeyValueService>();
            services.AddTransient<IHook, PreUpdateRowVersionHook>();

            return new EFCoreBuilder(services, typeof(TDbContext));
        }
    }

    public class EFCoreBuilder
    {
        public EFCoreBuilder(IServiceCollection services, Type contextType)
        {
            Services = services;
            ContextType = contextType;
        }

        public IServiceCollection Services { get; }
        public Type ContextType { get; }

        public EFCoreBuilder WithProtectionStore()
        {
            Services.AddSingleton<IProtectionStore, ProtectionStore>();
            return this;
        }
        public EFCoreBuilder WithTransactionOptions(Action<TransactionOptions> options)
        {
            Services.Configure(options);
            return this;
        }

        public EFCoreBuilder WithRowLevelSecurityHook<TUserId>() where TUserId : IEquatable<TUserId>
        {
            Services.AddTransient<IHook, PreInsertRowLevelSecurityHook<TUserId>>();
            return this;
        }

        public EFCoreBuilder WithTrackingHook<TUserId>() where TUserId : IEquatable<TUserId>
        {
            Services.AddTransient<IHook, PreInsertCreationTrackingHook<TUserId>>();
            Services.AddTransient<IHook, PreUpdateModificationTrackingHook<TUserId>>();
            return this;
        }

        public EFCoreBuilder WithTenancyHook<TTenantId>() where TTenantId : IEquatable<TTenantId>
        {
            Services.AddTransient<IHook, PreInsertTenantEntityHook<TTenantId>>();
            return this;
        }

        public EFCoreBuilder WithRowIntegrityHook()
        {
            Services.AddTransient<IHook, RowIntegrityHook>();
            return this;
        }

        public EFCoreBuilder WithDeletedEntityHook()
        {
            Services.AddTransient<IHook, PreDeleteDeletedEntityHook>();
            return this;
        }
    }
}