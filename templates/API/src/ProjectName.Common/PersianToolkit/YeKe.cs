using System;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.ReflectionToolkit;
using DNTPersianUtils.Core;

namespace ProjectName.Common.PersianToolkit
{
    public static class YeKe
    {
        public static void ApplyCorrectYeKeToProperties(this object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            var propertyInfos = obj.GetType().GetProperties(
                BindingFlags.Public | BindingFlags.Instance
            ).Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

            var propertyReflector = new PropertyReflector();

            foreach (var propertyInfo in propertyInfos)
            {
                var propName = propertyInfo.Name;
                var value = propertyReflector.GetValue(obj, propName);
                if (value == null) continue;
                
                var strValue = value.ToString();
                var newVal = strValue.ApplyCorrectYeKe();
                if (newVal == strValue)
                {
                    continue;
                }

                propertyReflector.SetValue(obj, propName, newVal);
            }
        }
    }
}