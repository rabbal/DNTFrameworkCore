using System;
using System.Linq;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.Licensing
{
    public static class LicenseExtensions
    {
        public static bool FeatureEnabled(this License license, bool requiresAll, params string[] featureNames)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));
            if (featureNames == null) throw new ArgumentNullException(nameof(featureNames));

            return !requiresAll
                ? featureNames.Any(license.FeatureEnabled)
                : featureNames.All(featureName => FeatureEnabled(license, featureName));
        }

        public static bool FeatureEnabled(this License license, string featureName)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));
            if (featureName == null) throw new ArgumentNullException(nameof(featureName));

            return license.Features.Any(f =>
                f.Name == featureName && f.Value.Equals("true", StringComparison.OrdinalIgnoreCase));
        }

        public static T FeatureValue<T>(this License license, string featureName) where T : struct
        {
            return license.FeatureValue(featureName).To<T>();
        }

        public static string FeatureValue(this License license, string featureName)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));
            if (featureName == null) throw new ArgumentNullException(nameof(featureName));

            var feature = license.Features.SingleOrDefault(f => f.Name == featureName);
            if (feature == null)
                throw new InvalidOperationException(
                    $"There is not any feature with name:{featureName} in this license.");

            return feature.Value;
        }

        public static string AttributeValue(this License license, string attributeName)
        {
            if (license == null) throw new ArgumentNullException(nameof(license));
            if (attributeName == null) throw new ArgumentNullException(nameof(attributeName));

            if (!license.Attributes.TryGetValue(attributeName, out var value))
                throw new InvalidOperationException(
                    $"There is not any attribute with name:{attributeName} in this license.");

            return value;
        }

        public static T AttributeValue<T>(this License license, string attributeName) where T : struct
        {
            return license.AttributeValue(attributeName).To<T>();
        }
    }
}