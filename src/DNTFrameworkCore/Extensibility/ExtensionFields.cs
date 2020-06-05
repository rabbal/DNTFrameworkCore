using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DNTFrameworkCore.Extensibility
{
    public static class ExtensionFields
    {
        private static readonly ConditionalWeakTable<object, Dictionary<string, object>> _fields =
            new ConditionalWeakTable<object, Dictionary<string, object>>();

        public static void ExtensionField<T>(this T instance, string name, object value) where T : class
        {
            var fields = _fields.GetOrCreateValue(instance);

            if (fields.ContainsKey(name))
                fields[name] = value;
            else
                fields.Add(name, value);
        }

        public static object ExtensionField(this object instance, string name)
        {
            return instance.ExtensionField<object>(name);
        }

        public static TValue ExtensionField<TValue>(this object instance, string name)
        {
            if (_fields.TryGetValue(instance, out var fields) && fields.ContainsKey(name))
                return (TValue) fields[name];
            return default;
        }
    }
}