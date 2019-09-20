using System.Threading.Tasks;

namespace DNTFrameworkCore.Tenancy
{
    public interface ITenantStore
    {
        Task<Tenant> FindTenantAsync(string tenantId);
    }
}