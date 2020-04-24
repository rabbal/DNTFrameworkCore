using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DNTFrameworkCore.Extensibility
{
    /// <summary>
    /// Use this provider when you need access ExtraProperties with TypeDescriptor.GetProperties(instance)
    /// </summary>
    public class ExtensionPropertyTypeDescriptionProvider<T> : TypeDescriptionProvider where T : class
    {
        private static readonly TypeDescriptionProvider Default =
            TypeDescriptor.GetProvider(typeof(T));

        public ExtensionPropertyTypeDescriptionProvider() : base(Default)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type instanceType, object instance)
        {
            var descriptor = base.GetTypeDescriptor(instanceType, instance);
            return instance == null ? descriptor : new ExtensionPropertyCustomTypeDescriptor(descriptor, instance);
        }

        private sealed class ExtensionPropertyCustomTypeDescriptor : CustomTypeDescriptor
        {
            private readonly IEnumerable<ExtensionPropertyDescriptor<T>> _instanceExtraProperties;

            public ExtensionPropertyCustomTypeDescriptor(ICustomTypeDescriptor defaultDescriptor, object instance)
                : base(defaultDescriptor)
            {
                _instanceExtraProperties = instance.ExtensionPropertyList<T>();
            }

            public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
            {
                var properties = new PropertyDescriptorCollection(null);

                foreach (PropertyDescriptor property in base.GetProperties(attributes))
                {
                    properties.Add(property);
                }

                foreach (var property in _instanceExtraProperties)
                {
                    properties.Add(property);
                }

                return properties;
            }

            public override PropertyDescriptorCollection GetProperties()
            {
                return GetProperties(null);
            }
        }
    }
}