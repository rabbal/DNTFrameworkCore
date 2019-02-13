using System;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.UI.Inputs
{
    [Serializable]
    public class LocalizableComboboxItem : ILocalizableComboboxItem
    {
        public string Value { get; set; }

        public LocalizedString DisplayText { get; set; }

        public LocalizableComboboxItem()
        {
            
        }

        public LocalizableComboboxItem(string value, LocalizedString displayText)
        {
            Value = value;
            DisplayText = displayText;
        }
    }
}