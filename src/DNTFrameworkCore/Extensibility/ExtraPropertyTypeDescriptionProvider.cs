using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DNTFrameworkCore.Extensibility
{
    /// <summary>
    /// Use this provider when you need access ExtraProperties with TypeDescriptor.GetProperties(instance)
    /// </summary>
    public class ExtraPropertyTypeDescriptionProvider<T> : TypeDescriptionProvider where T : class
    {
        private static readonly TypeDescriptionProvider Default =
            TypeDescriptor.GetProvider(typeof(T));

        public ExtraPropertyTypeDescriptionProvider() : base(Default)
        {
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type instanceType, object instance)
        {
            var descriptor = base.GetTypeDescriptor(instanceType, instance);
            return instance == null ? descriptor : new ExtraPropertyCustomTypeDescriptor(descriptor, instance);
        }

        private sealed class ExtraPropertyCustomTypeDescriptor : CustomTypeDescriptor
        {
            private readonly IEnumerable<ExtraPropertyDescriptor<T>> _instanceExtraProperties;

            public ExtraPropertyCustomTypeDescriptor(ICustomTypeDescriptor defaultDescriptor, object instance)
                : base(defaultDescriptor)
            {
                _instanceExtraProperties = instance.ExtraPropertyList<T>();
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