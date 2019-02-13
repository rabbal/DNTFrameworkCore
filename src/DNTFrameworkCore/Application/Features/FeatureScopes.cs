using System;

namespace DNTFrameworkCore.Application.Features
{
    [Flags]
    public enum FeatureScopes
    {
        /// <summary>
        /// This <see cref="Feature"/> can be enabled/disabled per edition.
        /// </summary>
        Edition = 1 << 0,

        /// <summary>
        /// This Feature<see cref="Feature"/> can be enabled/disabled per tenant.
        /// </summary>
        Tenant = 1 << 1,

        /// <summary>
        /// This <see cref="Feature"/> can be enabled/disabled per tenant and edition.
        /// </summary>
        All = Edition | Tenant
    }
}