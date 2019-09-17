namespace DNTFrameworkCore.Tenancy
{
    public class TenancyOptions
    {
        public bool IsEnabled { get; set; }
        public TenancyDatabaseStrategy DatabaseStrategy { get; set; } = TenancyDatabaseStrategy.Hybrid;
        public string HeadTenantConnectionString { get; set; }
        public string HeadTenantId { get; set; } = TenancyConstants.HeadTenantId;
        public string HeadTenantName { get; set; } = TenancyConstants.HeadTenantName;
    }
}