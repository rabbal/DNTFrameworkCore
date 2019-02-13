using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.Localization
{
    /// <summary>
    /// Represents a string that can be localized when needed.
    /// </summary>
    public interface ILocalizableString
    {
        /// <summary>
        /// Localizes the string in current culture.
        /// </summary>
        /// <param name="factory">StringLocalizer factory</param>
        /// <returns>Localized string</returns>
        string Localize(IStringLocalizerFactory factory);
    }
}