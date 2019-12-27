using DNTFrameworkCore.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Cryptography
{
    public static class DataProtectionExtensions
    {
        /// <summary>
        /// Configures the data protection system to persist keys to an EntityFrameworkCore store
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/> instance to modify.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IDataProtectionBuilder PersistKeysToStore(this IDataProtectionBuilder builder)
        {
            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(provider =>
            {
                var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlRepository = new XmlRepository(provider.GetRequiredService<IProtectionStore>());
                });
            });

            return builder;
        }
    }
}