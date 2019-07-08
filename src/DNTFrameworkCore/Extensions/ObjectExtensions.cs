using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace DNTFrameworkCore.Extensions
{
    public static class ObjectExtensions
    {
        public static string GetGenericTypeName(this object @object)
        {
            return @object.GetType().GetGenericTypeName();
        }

        /// <summary>
        /// Converts given object to a value or enum type using <see cref="Convert.ChangeType(object,TypeCode)"/> or <see cref="Enum.Parse(Type,string)"/> method.
        /// </summary>
        /// <param name="value">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object value)
            where T : struct
        {
            if (value == null) throw new ArgumentNullException(nameof(value));

            if (typeof(T) == typeof(Guid))
            {
                return (T) TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(value.ToString());
            }

            if (!typeof(T).IsEnum) return (T) Convert.ChangeType(value, typeof(T), CultureInfo.InvariantCulture);

            if (Enum.IsDefined(typeof(T), value))
            {
                return (T) Enum.Parse(typeof(T), value.ToString());
            }
            else
            {
                throw new ArgumentException($"Enum type undefined '{value}'.");
            }
        }

        /// <summary>
        /// Check if an item is in a list.
        /// </summary>
        /// <param name="item">Item to check</param>
        /// <param name="list">List of items</param>
        /// <typeparam name="T">Type of the items</typeparam>
        public static bool IsIn<T>(this T item, params T[] list)
        {
            return list.Contains(item);
        }
    }
}