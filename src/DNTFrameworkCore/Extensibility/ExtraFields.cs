using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace DNTFrameworkCore.Extensibility
{
    public static class ExtraFields
    {
        private static readonly ConditionalWeakTable<object, Dictionary<string, object>> FieldCache =
            new ConditionalWeakTable<object, Dictionary<string, object>>();

        public static void ExtraField<T>(this T instance, string name, object value) where T : class
        {
            var fields = FieldCache.GetOrCreateValue(instance);

            if (fields.ContainsKey(name))
                fields[name] = value;
            else
                fields.Add(name, value);
        }

        public static object ExtraField(this object instance, string name)
        {
            return instance.ExtraField<object>(name);
        }

        public static TValue ExtraField<TValue>(this object instance, string name)
        {
            if (FieldCache.TryGetValue(instance, out var fields) && fields.ContainsKey(name))
                return (TValue) fields[name];
            return default;
        }
    }
}