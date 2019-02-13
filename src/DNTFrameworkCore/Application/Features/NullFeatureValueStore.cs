using System.Threading.Tasks;

namespace DNTFrameworkCore.Application.Features
{
    public class NullFeatureValueStore : IFeatureValueStore
    {
              /// <summary>
        /// Gets the singleton instance.
        /// </summary>
        public static NullFeatureValueStore Instance { get; } = new NullFeatureValueStore();

        /// <inheritdoc/>
        public Task<string> ReadValueAsync(long tenantId, Feature feature)
        {
            return Task.FromResult((string) null);
        }
    }
}