namespace DNTFrameworkCore.TestTenancy.Tenancy
{
    public class TenantOptions
    {
        public IList<TenantInformation> Tenants { get; } = new List<TenantInformation>();
    }
    
    public class TenantInformation
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ISet<string> Hostnames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}