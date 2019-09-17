using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Tenancy
{
    public static class ServiceProviderExtensions
    {
        public static string GetTenantConnectionString(this IServiceProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            var session = provider.GetRequiredService<ITenantSession>();
            var configuration = provider.GetRequiredService<IConfiguration>();
            var options = provider.GetRequiredService<IOptions<TenancyOptions>>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (options.Value.IsEnabled &&
                options.Value.DatabaseStrategy != TenancyDatabaseStrategy.Shared)
            {
                connectionString = session.Tenant.ConnectionString;
            }

            if (string.IsNullOrEmpty(connectionString))
                throw new InvalidOperationException("Please set the DefaultConnection in appsettings.json file.");

            return connectionString;
        }
    }
}