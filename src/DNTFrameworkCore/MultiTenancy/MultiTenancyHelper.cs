using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.MultiTenancy
{
    internal class MultiTenancyHelper
    {
        public static bool IsTenantEntity(object entity)
        {
            return entity is ITenantEntity;
        }

        /// <summary>
        /// Check entity is owned by exceptedTenantId
        /// </summary>
        /// <param name="entity">The entity to check</param>
        /// <param name="expectedTenantId">TenantId or 1 for host</param>
        public static bool IsTenantEntity(object entity, long expectedTenantId)
        {
            return entity is ITenantEntity tenantEntity && tenantEntity.TenantId == expectedTenantId;
        }

        public static bool IsHostEntity(object entity)
        {
            var attribute = entity.GetType().GetTypeInfo()
                .GetCustomAttributes(typeof(MultiTenancySideAttribute), true)
                .Cast<MultiTenancySideAttribute>()
                .FirstOrDefault();

            return attribute != null && attribute.Side.HasFlag(MultiTenancySides.Host);
        }
    }
}