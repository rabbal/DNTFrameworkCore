using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.MultiTenancy
{
    public static class MultiTenancyHelper
    {
        public static bool IsHostEntity(this IEntity entity)
        {
            var attribute = FindMultiTenancySideAttribute(entity);

            return attribute != null && attribute.Side.HasFlag(MultiTenancySides.Host);
        }

        public static bool IsTenantEntity(this IEntity entity)
        {
            var attribute = FindMultiTenancySideAttribute(entity);

            return attribute != null && attribute.Side.HasFlag(MultiTenancySides.Tenant);
        }

        private static MultiTenancySideAttribute FindMultiTenancySideAttribute(object entity)
        {
            return entity.GetType().GetTypeInfo()
                .GetCustomAttributes(typeof(MultiTenancySideAttribute), true)
                .Cast<MultiTenancySideAttribute>()
                .FirstOrDefault();
        }
    }
}