using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Licensing
{
    public class LicenseFeature : ValueObject
    {
        private LicenseFeature()
        {
        }

        public string Name { get; private set; }
        public string DisplayName { get; private set; }
        public string Value { get; private set; }
        public string Description { get; private set; }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Name; }
        }

        public static LicenseFeature New(string name, string displayName, string value, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(displayName)) throw new ArgumentNullException(nameof(displayName));
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

            var feature = new LicenseFeature
            {
                Name = name,
                DisplayName = displayName,
                Value = value,
                Description = description
            };

            return feature;
        }
    }
}