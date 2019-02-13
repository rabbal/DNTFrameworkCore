using System;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.Localization
{
    /// <summary>
    /// A class that gets the same string on every localization.
    /// </summary>
    [Serializable]
    public class FixedLocalizableString : ILocalizableString
    {
        /// <summary>
        /// The fixed string.
        /// Whenever Localize methods called, this string is returned.
        /// </summary>
        public virtual string FixedString { get; }

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private FixedLocalizableString()
        {

        }

        /// <summary>
        /// Creates a new instance of <see cref="FixedLocalizableString"/>.
        /// </summary>
        /// <param name="fixedString">
        /// The fixed string.
        /// Whenever Localize methods called, this string is returned.
        /// </param>
        public FixedLocalizableString(string fixedString)
        {
            FixedString = fixedString;
        }

        public string Localize(IStringLocalizerFactory factory)
        {
            return FixedString;
        }
        
        public override string ToString()
        {
            return FixedString;
        }
    }
}