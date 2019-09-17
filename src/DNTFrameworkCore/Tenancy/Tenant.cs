using System.Collections.Generic;

namespace DNTFrameworkCore.Tenancy
{
    /// <summary>
    /// Tenant information
    /// </summary>
    public class Tenant
    {
        public Tenant()
        {
            Properties = new Dictionary<string, object>();
        }

        /// <summary>
        /// The tenant Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The tenant Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The tenant database ConnectionString
        /// </summary>
        public string ConnectionString { get; set; }

        /// <summary>
        /// Name of the culture like "fa" or "fa-IR"
        /// </summary>
        public string LanguageName { get; set; }

        /// <summary>
        /// The tenant TimeZoneId
        /// </summary>
        public string TimeZoneId { get; set; }

        /// <summary>
        /// The tenant ThemeName
        /// </summary>
        public string ThemeName { get; set; }

        /// <summary>
        /// Tenant items
        /// </summary>
        public IDictionary<string, object> Properties { get; }
    }
}