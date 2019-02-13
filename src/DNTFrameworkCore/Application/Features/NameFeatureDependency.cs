using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Application.Features
{
    internal class NameFeatureDependency : IFeatureDependency
    {
        /// <summary>
        /// A list of features to be checked if they are enabled.
        /// </summary>
        public string[] Features { get; set; }

        /// <summary>
        /// If this property is set to true, all of the <see cref="Features"/> must be enabled.
        /// If it's false, at least one of the <see cref="Features"/> must be enabled.
        /// Default: false.
        /// </summary>
        public bool RequiresAll { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameFeatureDependency"/> class.
        /// </summary>
        /// <param name="features">The features.</param>
        public NameFeatureDependency(params string[] features)
        {
            Features = features;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NameFeatureDependency"/> class.
        /// </summary>
        /// <param name="requiresAll">
        /// If this is set to true, all of the <see cref="Features"/> must be enabled.
        /// If it's false, at least one of the <see cref="Features"/> must be enabled.
        /// </param>
        /// <param name="features">The features.</param>
        public NameFeatureDependency(bool requiresAll, params string[] features)
            : this(features)
        {
            RequiresAll = requiresAll;
        }

        /// <inheritdoc/>
        public Task<bool> IsSatisfiedAsync(FeatureDependencyContext context)
        {
            return context.ServiceProvider.GetRequiredService<IFeatureChecker>().IsEnabledAsync(RequiresAll, Features);
        }
    }
}