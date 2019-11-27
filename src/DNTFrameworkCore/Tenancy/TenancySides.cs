using System;

namespace DNTFrameworkCore.Tenancy
{
    /// <summary>
    /// Represents sides in a multi tenancy application.
    /// </summary>
    [Flags]
    public enum TenancySides
    {
        None = 0,

        /// <summary>
        /// Tenant side.
        /// </summary>
        Tenant = 1,

        /// <summary>
        /// Host (tenancy owner) side.
        /// </summary>
        HeadTenant = 1 << 1
    }
}