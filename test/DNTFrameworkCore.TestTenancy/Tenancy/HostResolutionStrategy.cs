using System.Threading.Tasks;
using DNTFrameworkCore.Tenancy;
using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.TestTenancy.Tenancy
{
    /// <summary>
    /// Resolve the host to a tenant name
    /// </summary>
    public class HostResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _context;

        public HostResolutionStrategy(IHttpContextAccessor context)
        {
            _context = context;
        }
    
        /// <summary>
        /// Get the tenant name
        /// </summary>
        public Task<string> ResolveTenantNameAsync()
        {
            return Task.FromResult(_context.HttpContext.Request.Host.Value);
        }
    }
}