using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.Localization
{
    public class NullStringLocalizerFactory : IStringLocalizerFactory
    {
        public IStringLocalizer Create(Type resourceSource)
        {
            return NullStringLocalizer.Instance;
        }

        public IStringLocalizer Create(string baseName, string location)
        {
            return NullStringLocalizer.Instance;
        }
    }

    public class NullStringLocalizer : IStringLocalizer
    {
        public static IStringLocalizer Instance { get; } = new NullStringLocalizer();
        static NullStringLocalizer() { }

        public LocalizedString this[string name] => new LocalizedString(name, name);

        public LocalizedString this[string name, params object[] arguments] => new LocalizedString(name, name);

        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return new List<LocalizedString>();
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return this;
        }
    }
}