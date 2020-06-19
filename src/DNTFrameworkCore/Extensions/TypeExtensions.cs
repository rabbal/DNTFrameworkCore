using System;
using System.Linq;
using System.Reflection;

namespace DNTFrameworkCore.Extensions
{
    public static class TypeExtensions
    {
        public static bool HasAttribute<TAttribute>(this Type type, bool inherited = false)
        {
            return type.GetTypeInfo().GetCustomAttributes(true).OfType<TAttribute>().Any();
        }

        public static string GetGenericTypeName(this Type type)
        {
            string typeName;

            if (type.IsGenericType)
            {
                var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());
                typeName = $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
            }
            else
            {
                typeName = type.Name;
            }

            return typeName;
        }
    }
}