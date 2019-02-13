using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.TestAPI.Resources
{
    public class MessagesResource
    {
    }

    public interface IMessageLocalizer : IStringLocalizer
    {
    }

    public class MessageLocalizer : IMessageLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public MessageLocalizer(IStringLocalizer<MessagesResource> localizer)
        {
            _localizer = localizer;
        }

        public LocalizedString this[string name, params object[] arguments] => _localizer[name, arguments];
        public LocalizedString this[string name] => _localizer[name];
        public IEnumerable<LocalizedString> GetAllStrings(bool includeParentCultures)
        {
            return _localizer.GetAllStrings(includeParentCultures);
        }

        public IStringLocalizer WithCulture(CultureInfo culture)
        {
            return _localizer.WithCulture(culture);
        }
    }
}