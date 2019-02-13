using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.MultiTenancy
{
    /// <summary>
    /// Extension methods for multi-tenancy.
    /// </summary>
    public static class MultiTenancyExtensions
    {
        /// <summary>
        /// Gets multi-tenancy side (<see cref="MultiTenancySides"/>) of an object that implements <see cref="ITenantEntity"/>.
        /// </summary>
        /// <param name="entity">The object</param>
        public static MultiTenancySides FindMultiTenancySide(this ITenantEntity entity)
        {
            return entity.TenantId == 0
                ? MultiTenancySides.Host
                : MultiTenancySides.Tenant;
        }
    }
}