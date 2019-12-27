namespace DNTFrameworkCore.TestTenancy.Tenancy
{
    /// <summary>
    /// Resolve the host to a tenant name
    /// </summary>
    public class HostResolutionStrategy : ITenantResolutionStrategy
    {
        private readonly IHttpContextAccessor _context;
        private readonly IOptions<TenantOptions> _options;

        public HostResolutionStrategy(IHttpContextAccessor context, IOptions<TenantOptions> options)
        {
            _context = context;
            _options = options;
        }

        /// <summary>
        /// Get the tenant name
        /// </summary>
        public string ResolveTenantId()
        {
            var host = _context.HttpContext.Request.Host.Value;

            return _options.Value.Tenants.FirstOrDefault(t => t.Hostnames.Contains(host))?.Id;
        }
    }
}