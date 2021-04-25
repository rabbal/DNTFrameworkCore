using System;
using System.Reflection;

namespace DNTFrameworkCore.Reflection
{
    public static class FastReflectionExtensions
    {
        public static FastPropertyInfo[] GetFastProperties(this Type type)
        {
            return FastReflection.Instance.GetProperties(type);
        }

        public static FastFieldInfo[] GetFastFields(this Type type)
        {
            return FastReflection.Instance.GetFields(type);
        }

        public static void SetFastValue(this PropertyInfo property, object instance, object value)
        {
            var propertyInfo = FastReflection.Instance.GetProperty(property);
            if (propertyInfo.CanWrite) propertyInfo.SetValue(instance, value);
            else throw new InvalidOperationException($"Can't set value for property: `{propertyInfo.Name}`");
        }

        public static object GetFastValue(this PropertyInfo property, object instance)
        {
            var propertyInfo = FastReflection.Instance.GetProperty(property);
            if (propertyInfo.CanRead) return propertyInfo.GetValue(instance);
            throw new InvalidOperationException($"Can't get value of property: `{propertyInfo.Name}`");
        }
    }
}