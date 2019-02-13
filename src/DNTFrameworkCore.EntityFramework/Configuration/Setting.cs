using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.EntityFramework.Configuration
{
    public class Setting : ModificationTrackingEntity
    {
        public Setting(long? tenantId, long? userId, string name, string value)
        {
            TenantId = tenantId;
            UserId = userId;
            Name = name;
            Value = value;
        }

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
        public string Name { get; set; }

        /// <summary>
        /// Value of the setting.
        /// </summary>
        public string Value { get; set; }
    }
}