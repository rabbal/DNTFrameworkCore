using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Exceptions;

namespace DNTFrameworkCore.Application.Features
{
    public static class FeatureCheckerExtensions
    {
        /// <summary>
        /// Checks if given feature is enabled.
        /// This should be used for boolean-value features.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="featureName">Unique feature name</param>
        /// <returns>True, if current feature's value is "true".</returns>
        public static async Task<bool> IsEnabledAsync(this IFeatureChecker featureChecker, string featureName)
        {
            return string.Equals(await featureChecker.ReadValueAsync(featureName), "true", StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Used to check if one of all given features are enabled.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="requiresAll">True, to require all given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Name of the features</param>
        public static async Task<bool> IsEnabledAsync(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return true;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!await featureChecker.IsEnabledAsync(featureName))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                foreach (var featureName in featureNames)
                {
                    if (await featureChecker.IsEnabledAsync(featureName))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
     
        /// <summary>
        /// Checks if given feature is enabled. Throws <see cref="FrameworkException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="featureName">Unique feature name</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, string featureName)
        {
            if (!await featureChecker.IsEnabledAsync(featureName))
            {
                throw new FrameworkException("Feature is not enabled: " + featureName);
            }
        }

        /// <summary>
        /// Checks if one of all given features are enabled. Throws <see cref="FrameworkException"/> if not.
        /// </summary>
        /// <param name="featureChecker"><see cref="IFeatureChecker"/> instance</param>
        /// <param name="requiresAll">True, to require all given features are enabled. False, to require one or more.</param>
        /// <param name="featureNames">Name of the features</param>
        public static async Task CheckEnabledAsync(this IFeatureChecker featureChecker, bool requiresAll, params string[] featureNames)
        {
            if (featureNames.IsNullOrEmpty())
            {
                return;
            }

            if (requiresAll)
            {
                foreach (var featureName in featureNames)
                {
                    if (!await featureChecker.IsEnabledAsync(featureName))
                    {
                        throw new FrameworkException(
                            "Required features are not enabled. All of these features must be enabled: " +
                            string.Join(", ", featureNames)
                            );
                    }
                }
            }
            else
            {
                foreach (var featureName in featureNames)
                {
                    if (await featureChecker.IsEnabledAsync(featureName))
                    {
                        return;
                    }
                }

                throw new FrameworkException(
                    "Required features are not enabled. At least one of these features must be enabled: " +
                    string.Join(", ", featureNames)
                    );
            }
        }
    }
}