using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Linq;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Protection
{
    internal sealed class ProtectionStore : IProtectionStore
    {
        private readonly IServiceProvider _provider;

        public ProtectionStore(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IReadOnlyCollection<XElement> FetchElements()
        {
            return _provider.RunScoped<ReadOnlyCollection<XElement>, IUnitOfWork>(uow =>
            {
                return uow.Set<ProtectionKey>().AsNoTracking().Select(k => XElement.Parse(k.XmlValue)).ToList()
                    .AsReadOnly();
            });
        }

        public void SaveElement(XElement element, string friendlyName)
        {
            // We need a separate context to call its SaveChanges several times,
            // without using the current request's context and changing its internal state.
            _provider.RunScoped<IUnitOfWork>(uow =>
            {
                var keys = uow.Set<ProtectionKey>();
                var entity = keys.SingleOrDefault(k => k.FriendlyName == friendlyName);
                if (entity != null)
                {
                    entity.XmlValue = element.ToString(SaveOptions.DisableFormatting);
                    keys.Update(entity);
                }
                else
                {
                    keys.Add(new ProtectionKey
                    {
                        FriendlyName = friendlyName,
                        XmlValue = element.ToString(SaveOptions.DisableFormatting)
                    });
                }

                uow.SaveChanges();
            });
        }
    }
}