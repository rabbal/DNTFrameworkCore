using System.Collections.Generic;

namespace DNTFrameworkCore.UI.Inputs
{
    public interface ILocalizableComboboxItemSource
    {
        ICollection<ILocalizableComboboxItem> Items { get; }
    }
}