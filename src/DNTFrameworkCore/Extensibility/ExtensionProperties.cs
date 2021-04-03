using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace DNTFrameworkCore.Extensibility
{
    public static class ExtensionProperties
    {
        private static readonly ConditionalWeakTable<object, List<ExtraPropertyInfo>> _properties = new();

        private sealed class ExtraPropertyInfo
        {
            public string PropertyName { set; get; }
            public Type PropertyType { set; get; }
            public Func<object, object> PropertyValueFunc { get; set; }
            public Action<object, object> SetPropertyValueFunc { get; set; }
            public Attribute[] Attributes { get; set; }
        }

        public static void ExtensionProperty<T>(this T instance, string propertyName, object propertyValue) where T : class
        {
            instance.ExtensionProperty(propertyName, _ => propertyValue, propertyValue.GetType());
        }

        public static void ExtensionProperty<T>(this T instance, string propertyName, Func<T, object> propertyValueFunc,
            Type propertyType, Action<T, object> setPropertyValueFunc = null, Attribute[] attributes = null)
            where T : class
        {
            var properties = _properties.GetOrCreateValue(instance);

            var property = properties.Find(p => p.PropertyName == propertyName);
            if (property != null)
                property.PropertyValueFunc = obj => propertyValueFunc(obj as T);
            else
            {
                property = new ExtraPropertyInfo
                {
                    PropertyName = propertyName,
                    PropertyType = propertyType,
                    PropertyValueFunc = _ => propertyValueFunc(_ as T),
                    Attributes = attributes
                };

                if (setPropertyValueFunc != null)
                {
                    property.SetPropertyValueFunc = (obj, value) => setPropertyValueFunc(obj as T, value);
                }

                properties.Add(property);
            }
        }

        public static TValue ExtensionProperty<TValue>(this object instance, string name)
        {
            if (!_properties.TryGetValue(instance, out var properties)) return default;

            var property = properties.Find(p => p.PropertyName == name);
            if (property == null) return default;

            return (TValue) property.PropertyValueFunc(instance);
        }

        public static object ExtensionProperty(this object instance, string name)
        {
            return instance.ExtensionProperty<object>(name);
        }

        public static IEnumerable<ExtensionPropertyDescriptor<T>> ExtensionPropertyList<T>(this object instance) where T : class
        {
            if (!_properties.TryGetValue(instance, out var properties))
                throw new KeyNotFoundException($"key: {instance.GetType().Name} was not found in dictionary");

            return properties.Select(p =>
                new ExtensionPropertyDescriptor<T>(p.PropertyName, p.PropertyValueFunc, p.SetPropertyValueFunc,
                    p.PropertyType,
                    p.Attributes));
        }
    }
}