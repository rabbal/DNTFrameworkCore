using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DNTFrameworkCore.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Getting string attribute of Enum's value.
        /// Processing order is checking DisplayAttribute first and then DescriptionAttribute.
        /// If none of these is available, value.ToString() will be returned.
        /// </summary>
        /// <param name="flags">enum value</param>
        /// <returns>string attribute of Enum's value</returns>
        public static string GetStringValue(this Enum flags)
        {
            if (flags.IsFlags())
            {
                var text = GetEnumFlagsText(flags);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    return text;
                }
            }
            return GetEnumValueText(flags);   
        }

        public static bool IsFlags(this Enum flags)
        {
            return flags.GetType().GetTypeInfo().GetCustomAttributes(true).OfType<FlagsAttribute>().Any();
        }

        private static string GetEnumFlagsText(Enum flags)
        {
            const char leftToRightSeparator = ',';
            const char rightToRightSeparator = '،';

            var sb = new StringBuilder();
            var items = Enum.GetValues(flags.GetType());
            foreach (var value in items)
            {
                if (!flags.HasFlag((Enum) value) || Convert.ToInt64((Enum) value) == 0) continue;
                
                var text = GetEnumValueText((Enum)value);
                var separator = text.ContainsRtlText() ? rightToRightSeparator : leftToRightSeparator;
                sb.Append(text).Append(separator).Append(" ");
            }

            return sb.ToString().Trim().TrimEnd(leftToRightSeparator).TrimEnd(rightToRightSeparator);
        }

        private static string GetEnumValueText(Enum value)
        {
            var text = value.ToString();
            var info = value.GetType().GetField(text);

            var display = info?.GetCustomAttributes(true).OfType<DisplayAttribute>().FirstOrDefault();
            if (display != null)
            {
                return display.Name;
            }
            
            var description = info?.GetCustomAttributes(true).OfType<DescriptionAttribute>().FirstOrDefault();
            if (description != null)
            {
                return description.Description;
            }

            return text;
        }

    }
}