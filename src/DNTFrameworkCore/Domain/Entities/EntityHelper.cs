using System;

namespace DNTFrameworkCore.Domain.Entities
{
    public static class EntityHelper
    {
        public static bool IsEntity(this Type type)
        {
            return typeof(IEntity).IsAssignableFrom(type);
        }
    }
}