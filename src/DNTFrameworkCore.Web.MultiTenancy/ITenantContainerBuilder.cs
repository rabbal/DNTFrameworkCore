using System.Threading.Tasks;
using DNTFrameworkCore.MultiTenancy;
using StructureMap;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    public interface ITenantContainerBuilder
    {
        Task<IContainer> BuildAsync(TenantInfo tenant);
    }
}
