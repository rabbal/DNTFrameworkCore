using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using DNTFrameworkCore.Cryptography;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.DataProtection.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.EFCore.Cryptography
{
    public static class DataProtectionExtensions
    {
        /// <summary>
        /// Configures the data protection system to persist keys to an EntityFrameworkCore store
        /// </summary>
        /// <param name="builder">The <see cref="IDataProtectionBuilder"/> instance to modify.</param>
        /// <returns>The value <paramref name="builder"/>.</returns>
        public static IDataProtectionBuilder PersistKeysToDbContext<TContext>(this IDataProtectionBuilder builder)
            where TContext : DbContext
        {
            builder.Services.AddSingleton<IConfigureOptions<KeyManagementOptions>>(provider =>
            {
                var loggerFactory = provider.GetService<ILoggerFactory>() ?? NullLoggerFactory.Instance;
                return new ConfigureOptions<KeyManagementOptions>(options =>
                {
                    options.XmlRepository = new XmlRepository<TContext>(provider, loggerFactory);
                });
            });

            return builder;
        }
    }

    /// <summary>
    /// An <see cref="IXmlRepository"/> backed by an EntityFrameworkCore datastore.
    /// </summary>
    public class XmlRepository<TContext> : IXmlRepository
        where TContext : DbContext
    {
        private readonly IServiceProvider _provider;
        private readonly ILogger _logger;

        public XmlRepository(IServiceProvider provider, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _logger = loggerFactory.CreateLogger<XmlRepository<TContext>>();
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<XElement> GetAllElements()
        {
            using (var scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                return context.Set<ProtectionKey>().AsNoTracking().Select(key => TryParseKeyXml(key.XmlValue)).ToList()
                    .AsReadOnly();
            }
        }

        /// <inheritdoc />
        public void StoreElement(XElement element, string friendlyName)
        {
            using (var scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TContext>();
                var key = new ProtectionKey
                {
                    FriendlyName = friendlyName,
                    XmlValue = element.ToString(SaveOptions.DisableFormatting)
                };

                context.Set<ProtectionKey>().Add(key);
                _logger.LogSavingKeyToDbContext(friendlyName, typeof(TContext).Name);
                context.SaveChanges();
            }
        }

        private XElement TryParseKeyXml(string xml)
        {
            try
            {
                return XElement.Parse(xml);
            }
            catch (Exception e)
            {
                _logger?.LogExceptionWhileParsingKeyXml(xml, e);
                return null;
            }
        }
    }
}