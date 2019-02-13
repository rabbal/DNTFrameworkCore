using System.Collections.Generic;
using System.Globalization;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.TestAPI.Resources
{
    public class LabelsResource
    {

    }

    public interface ILabelLocalizer : IStringLocalizer
    {
    }

    public class LabelLocalizer : ILabelLocalizer
    {
        private readonly IStringLocalizer _localizer;

        public LabelLocalizer(IStringLocalizer<LabelsResource> localizer)
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
