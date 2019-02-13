using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace DNTFrameworkCore.Extensions
{
    public static class PropertyDataAnnotationExtensions
    {
        /// <summary>
        /// Getting string attribute of Enum's value.
        /// Processing order is checking DisplayNameAttribute first and then DescriptionAttribute.
        /// If none of these is available, value.ToString() will be returned.
        /// </summary>
        /// <param name="value">enum value</param>
        /// <returns>string attribute of Enum's value</returns>
        public static string GetEnumStringValue(this Enum value)
        {
            var info = value.GetType().GetField(value.ToString());

            var displayName = info.GetCustomAttributes(true).OfType<DisplayNameAttribute>().FirstOrDefault();
            if (displayName != null) return displayName.DisplayName;

            var description = info.GetCustomAttributes(true).OfType<DescriptionAttribute>().FirstOrDefault();
            if (description != null) return description.Description;

            return value.ToString();
        }

        /// <summary>
        /// Returns DisplayFormatAttribute data.
        /// </summary>
        /// <param name="info">Property metadata info</param>
        /// <returns>NullDisplayText</returns>
        public static string GetNullDisplayTextAttribute(this MemberInfo info)
        {
            var displayFormat = info.GetCustomAttributes(true).OfType<DisplayFormatAttribute>().FirstOrDefault();
            return displayFormat == null ? string.Empty : displayFormat.NullDisplayText;
        }
    }
}