using DNTFrameworkCore.Localization;
using Microsoft.Extensions.Localization;
using Newtonsoft.Json;

namespace DNTFrameworkCore.UI.Inputs
{
    public interface ILocalizableComboboxItem
    {
        string Value { get; set; }

        [JsonConverter(typeof(LocalizableStringToStringJsonConverter))]
        LocalizedString DisplayText { get; set; }
    }
}