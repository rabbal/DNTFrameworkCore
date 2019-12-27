namespace DNTFrameworkCore.TestTenancy.Tenancy
{
    /// <summary>
    /// In memory store for testing
    /// </summary>
    public class InMemoryTenantStore : ITenantStore
    {
        private readonly IOptions<TenantOptions> _options;

        public InMemoryTenantStore(IOptions<TenantOptions> options)
        {
            _options = options;
        }

        /// <summary>
        /// Get a tenant for a given tenantName
        /// </summary>
        /// <param name="tenantId"></param>
        /// <returns></returns>
        public Task<Tenant> FindTenantAsync(string tenantId)
        {
            var option = _options.Value.Tenants.FirstOrDefault(t => t.Id == tenantId);

            if (option == null) return null;

            var tenant = new Tenant
            {
                Id = option.Id,
                Name = option.Name,
                ConnectionString = "", //Todo
                TimeZoneId = "", //Todo
                ThemeName = "Material", //Todo
                LanguageName = "fa-IR"
            };

            tenant.Properties["PropertyName"] = "PropertyValue";

            return Task.FromResult(tenant);
        }
    }
}