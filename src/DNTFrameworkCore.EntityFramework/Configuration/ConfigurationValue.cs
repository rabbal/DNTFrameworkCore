using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.EntityFramework.Configuration
{
    public class ConfigurationValue : ModificationTrackingEntity
    {
        /// <summary>
        /// TenantId for this setting.
        /// TenantId is Null if this setting is not Tenant level.
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// UserId for this setting.
        /// UserId is null if this setting is not user level.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Unique name of the setting.
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Value of the setting.
        /// </summary>
        public string Value { get; set; }
    }
}
