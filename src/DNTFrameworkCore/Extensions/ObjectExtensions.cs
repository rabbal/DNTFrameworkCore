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
        /// Converts given object to a value type using <see cref="Convert.ChangeType(object,System.TypeCode)"/> method.
        /// </summary>
        /// <param name="obj">Object to be converted</param>
        /// <typeparam name="T">Type of the target object</typeparam>
        /// <returns>Converted object</returns>
        public static T To<T>(this object obj)
            where T : struct
        {
            if (typeof(T) == typeof(Guid))
            {
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFromInvariantString(obj.ToString());
            }

            return (T)Convert.ChangeType(obj, typeof(T), CultureInfo.InvariantCulture);
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

        //Todo: move to PersianToolkit library
//        public static void ApplyCorrectYeKeToProperties(this object obj)
//        {
//            if (obj == null)
//            {
//                throw new ArgumentNullException(nameof(obj));
//            }
//
//            var propertyInfos = obj.GetType().GetProperties(
//                BindingFlags.Public | BindingFlags.Instance
//            ).Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));
//
//            var propertyReflector = new PropertyReflector();
//
//            foreach (var propertyInfo in propertyInfos)
//            {
//                var propName = propertyInfo.Name;
//                var value = propertyReflector.GetValue(obj, propName);
//                if (value != null)
//                {
//                    var strValue = value.ToString();
//                    var newVal = strValue.ApplyCorrectYeKe();
//                    if (newVal == strValue)
//                    {
//                        continue;
//                    }
//
//                    propertyReflector.SetValue(obj, propName, newVal);
//                }
//            }
//        }
    }
}