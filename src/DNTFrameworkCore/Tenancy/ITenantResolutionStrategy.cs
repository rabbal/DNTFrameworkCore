using System.Threading.Tasks;

namespace DNTFrameworkCore.Tenancy
{
    public interface ITenantResolutionStrategy
    {
        Task<string> ResolveTenantNameAsync();
    }
}