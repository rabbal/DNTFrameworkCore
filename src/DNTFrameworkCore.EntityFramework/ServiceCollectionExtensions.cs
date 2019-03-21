using System;
using DNTFrameworkCore.Auditing;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.EntityFramework.Application;
using DNTFrameworkCore.EntityFramework.Auditing;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.EntityFramework.Context.Internal;
using DNTFrameworkCore.EntityFramework.DataProtection;
using DNTFrameworkCore.EntityFramework.Transaction;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Transaction;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.EntityFramework
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDNTAuditingStore<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.Replace(ServiceDescriptor.Transient(typeof(IAuditingStore), typeof(AuditLogStore<TDbContext>)));
        }

        public static void AddDNTProtectionRepository<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContext
        {
            services.AddSingleton<IDataProtectionRepository, DataProtectionRepository<TDbContext>>();
        }

        public static string ReadTenantConnectionString(this IServiceProvider provider)
        {
            var tenant = provider.GetService<ITenant>();
            var configuration = provider.GetService<IConfiguration>();
            var multiTenancy = provider.GetService<IOptions<MultiTenancyConfiguration>>();

            var connectionString =
                configuration.GetConnectionString("DefaultConnection");

            if (multiTenancy.Value.Enabled &&
                multiTenancy.Value.DatabaseStrategy != MultiTenancyDatabaseStrategy.SingleDatabase &&
                tenant.Value != null)
            {
                connectionString = tenant.Value.ConnectionString;
            }

            if (connectionString.IsEmpty())
                throw new InvalidOperationException("Please set the DefaultConnection in appsettings.json file.");

            return connectionString;
        }

        public static void AddDNTUnitOfWork<TDbContext>(this IServiceCollection services)
            where TDbContext : DbContextCore
        {
            services.AddScoped(provider => (IUnitOfWork) provider.GetRequiredService(typeof(TDbContext)));
            services.AddScoped<ITransactionProvider, TransactionProvider<TDbContext>>();
            services.AddScoped<IHookEngine, HookEngine>();

            services.AddTransient<IPreActionHook, TrackingPreInsertHook>();
            services.AddTransient<IPreActionHook, TrackingPreUpdateHook>();
            services.AddTransient<IPreActionHook, TrackingPreDeleteHook>();
            services.AddTransient<IPreActionHook, RowVersionPreUpdateHook>();
            services.AddTransient<IPreActionHook, SoftDeletePreDeleteHook>();
            services.AddTransient<IPreActionHook, RowLevelSecurityPreInsertHook>();
            services.AddTransient<IPreActionHook, ApplyCorrectYeKeHookPreInsertHook>();
            services.AddTransient<IPreActionHook, ApplyCorrectYeKeHookPreUpdateHook>();
        }
    }
}