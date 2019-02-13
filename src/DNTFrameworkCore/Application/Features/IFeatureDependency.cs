using System.Threading.Tasks;

namespace DNTFrameworkCore.Application.Features
{
    public interface IFeatureDependency
    {
        /// <summary>
        /// Checks depended features and returns true if dependencies are satisfied.
        /// </summary>
        Task<bool> IsSatisfiedAsync(FeatureDependencyContext context);
    }
}