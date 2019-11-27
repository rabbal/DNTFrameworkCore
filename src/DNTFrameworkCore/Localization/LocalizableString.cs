using System;
using DNTFrameworkCore.GuardToolkit;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.Localization
{
    /// <summary>
    /// Represents a string that can be localized.
    /// </summary>
    [Serializable]
    public class LocalizableString : ILocalizableString
    {
        /// <summary>
        /// The localization to load navigation resources from.
        /// Can be Null for Database localization source.
        /// </summary>
        /// <returns></returns>
        public string ResourceLocation { get; }

        /// <summary>
        /// Unique name of the localization resource like, SharedResource,...
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// Unique Name of the string to be localized.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Needed for serialization.
        /// </summary>
        private LocalizableString()
        {
        }

        public LocalizableString(string name, string resourceName, string resourceLocation = null)
        {
            Name = Ensure.IsNotNullOrEmpty(name, nameof(name));
            ResourceName = Ensure.IsNotNullOrEmpty(resourceName, nameof(resourceName));
            ResourceLocation = resourceLocation;
        }

        public string Localize(IStringLocalizerFactory factory)
        {
            return factory.Create(ResourceName, ResourceLocation).GetString(Name);
        }

        public override string ToString()
        {
            return $"[LocalizableString: {Name}, {ResourceName}]";
        }
    }
}