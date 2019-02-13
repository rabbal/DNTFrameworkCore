using System.Threading.Tasks;
using DNTFrameworkCore.MultiTenancy;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    public interface ITenantResolver
    {
        Task<TenantInfo> ResolveAsync(HttpContext context);
    }
}