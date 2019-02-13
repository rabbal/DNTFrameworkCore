using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Application.Features
{
    public interface IFeatureValueStore : IScopedDependency
    {
        /// <summary>
        /// Gets the feature value or null.
        /// </summary>
        /// <param name="tenantId">The tenant id.</param>
        /// <param name="feature">The feature.</param>
        Task<string> ReadValueAsync(long tenantId, Feature feature);
    }
}