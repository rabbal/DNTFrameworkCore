namespace DNTFrameworkCore.MultiTenancy
{
    /// <summary>
    /// Represents Current Tenant's Information 
    /// </summary>
    public class TenantInfo
    {
        public TenantInfo(long id, string name, string connectionString)
        {
            Id = id;
            Name = name;
            ConnectionString = connectionString;
        }

        public long Id { get; }
        public string Name { get; }
        public string ConnectionString { get; }

        /// <summary>
        /// Name of the culture like "fa" or "fa-IR"
        /// </summary>
        public string LanguageName { get; set; }

        public string TimeZoneId { get; set; }
        public string Theme { get; set; }
    }
}