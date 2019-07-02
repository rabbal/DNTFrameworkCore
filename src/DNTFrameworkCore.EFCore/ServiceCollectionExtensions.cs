using System;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.EFCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.Protection;
using DNTFrameworkCore.EFCore.Transaction;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.EFCore
{
    public static class ServiceCollectionExtensions
    {
        public static string ReadTenantConnectionString(this IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            var tenant = provider.GetService<ITenant>();
            var configuration = provider.GetService<IConfiguration>();
            var multiTenancy = provider.GetService<IOptions<MultiTenancyOptions>>();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (multiTenancy.Value.Enabled &&
                multiTenancy.Value.DatabaseStrategy != MultiTenancyDatabaseStrategy.SingleDatabase &&
                tenant.HasValue)
            {
                connectionString = tenant.Value.ConnectionString;
            }

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Please set the DefaultConnection in appsettings.json file.");

            return connectionString;
        }

        // ReSharper disable once InconsistentNaming
        public static void AddEFCore<TDbContext>(this IServiceCollection services,
            Action<TransactionOptions> setupAction = null)
            where TDbContext : DbContext, IUnitOfWork
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddScoped(provider => (IUnitOfWork) provider.GetRequiredService(typeof(TDbContext)));
            services.AddTransient<TransactionInterceptor>();
            if (setupAction != null)
            {
                services.Configure(setupAction);
            }

            services.AddScoped<IConfigurationValueService, ConfigurationValueService>();

            services.AddSingleton<IProtectionRepository, ProtectionRepository<TDbContext>>();
            services.AddScoped<IHookEngine, HookEngine>();
            services.AddTransient<IPreActionHook, PreInsertCreationTrackingHook>();
            services.AddTransient<IPreActionHook, PreInsertTenantEntityHook>();
            services.AddTransient<IPreActionHook, PreInsertHasRowLevelSecurityHook>();
            services.AddTransient<IPreActionHook, PreUpdateModificationTrackingHook>();
            services.AddTransient<IPreActionHook, PreDeleteSoftDeleteEntityHook>();
            services.AddTransient<IPreActionHook, PreUpdateRowVersionHook>();
        }
    }
}