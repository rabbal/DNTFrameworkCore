using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.DataProtection
{
    public class DataProtectionRepository<TContext> : IDataProtectionRepository
        where TContext : DbContext
    {
        private readonly IServiceProvider _provider;

        public DataProtectionRepository(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IReadOnlyCollection<XElement> RetrieveElements()
        {
            return _provider.RunScoped<ReadOnlyCollection<XElement>, TContext>(context =>
            {
                var dataProtectionKeys = context.Set<DataProtectionKey>();
                return new ReadOnlyCollection<XElement>(dataProtectionKeys.Select(k => XElement.Parse(k.XmlValue))
                    .ToList());
            });
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            // We need a separate context to call its SaveChanges several times,
            // without using the current request's context and changing its internal state.
            _provider.RunScoped<TContext>(context =>
            {
                var dataProtectionKeys = context.Set<DataProtectionKey>();
                var entity = dataProtectionKeys.SingleOrDefault(k => k.FriendlyName == friendlyName);
                if (null != entity)
                {
                    entity.XmlValue = element.ToString();
                    dataProtectionKeys.Update(entity);
                }
                else
                {
                    dataProtectionKeys.Add(new DataProtectionKey
                    {
                        FriendlyName = friendlyName,
                        XmlValue = element.ToString()
                    });
                }

                context.SaveChanges();
            });
        }
    }
}