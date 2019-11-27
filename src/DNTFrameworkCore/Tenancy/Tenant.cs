using System.Collections.Generic;

namespace DNTFrameworkCore.Tenancy
{
    /// <summary>
    /// Tenant information
    /// </summary>
    public sealed class Tenant
    {
        private int? _hashCode;

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

        public override int GetHashCode()
        {
            if (!_hashCode.HasValue)
                _hashCode = Id.GetHashCode() ^ 31; // XOR for random distribution

            return _hashCode.Value;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Tenant tenant)) return false;

            return ReferenceEquals(this, tenant) || Id.Equals(tenant.Id);
        }

        public override string ToString()
        {
            return $"[{Name} : {Id}]";
        }

        public static bool operator ==(Tenant left, Tenant right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Tenant left, Tenant right)
        {
            return !(left == right);
        }
    }
}