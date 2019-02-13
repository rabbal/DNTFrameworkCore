using System;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.UI.Inputs
{
    /// <summary>
    /// Combobox value UI type.
    /// </summary>
    [Serializable]
    [InputType("COMBOBOX")]
    public class ComboboxInputType : InputTypeBase
    {
        public ILocalizableComboboxItemSource ItemSource { get; set; }

        public ComboboxInputType()
        {
            
        }

        public ComboboxInputType(ILocalizableComboboxItemSource itemSource)
        {
            ItemSource = itemSource;
        }

        public ComboboxInputType(ILocalizableComboboxItemSource itemSource, IValueValidator validator)
            : base(validator)
        {
            ItemSource = itemSource;
        }
    }
}