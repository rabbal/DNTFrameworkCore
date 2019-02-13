namespace DNTFrameworkCore.MultiTenancy
{
    /// <summary>
    /// Represents Current Tenant's Information 
    /// </summary>
    public class TenantInfo
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string ConnectionString { get; set; }
        /// <summary>
        /// Name of the culture like "fa" or "fa-IR"
        /// </summary>
        public string LanguageName { get; set; }
        public string TimeZoneId { get; set; }
        public string Theme { get; set; }
    }
}