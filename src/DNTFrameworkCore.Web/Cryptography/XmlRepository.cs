using System;
using System.Collections.Generic;
using System.Xml.Linq;
using DNTFrameworkCore.Cryptography;
using Microsoft.AspNetCore.DataProtection.Repositories;

namespace DNTFrameworkCore.Web.Cryptography
{
    internal sealed class XmlRepository : IXmlRepository
    {
        private readonly IProtectionStore _store;

        public XmlRepository(IProtectionStore store)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
        }

        public IReadOnlyCollection<XElement> GetAllElements()
        {
            return _store.FetchElements();
        }

        public void StoreElement(XElement element, string friendlyName)
        {
            _store.SaveElement(element, friendlyName);
        }
    }
}