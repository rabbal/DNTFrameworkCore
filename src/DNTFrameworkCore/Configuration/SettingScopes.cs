using System;

namespace DNTFrameworkCore.Configuration
{
    [Flags]
    public enum SettingScopes
    {
        /// <summary>
        /// Represents a Configuration that can be configured/changed for the application level.
        /// </summary>
        Application = 1 << 0,

        /// <summary>
        /// Represents a Configuration that can be configured/changed for each Tenant.
        /// This is reserved
        /// </summary>
        Tenant = 1 << 1,

        /// <summary>
        /// Represents a Configuration that can be configured/changed for each User.
        /// </summary>
        User = 1 << 2,

        /// <summary>
        /// Represents a Configuration that can be configured/changed for all levels
        /// </summary>
        All = Application | Tenant | User
    }
}