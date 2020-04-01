using System;
using System.ComponentModel;

namespace DNTFrameworkCore.Extensibility
{
    public sealed class ExtensionPropertyDescriptor<T> : PropertyDescriptor where T : class
    {
        private readonly Func<object, object> _propertyValueFunc;
        private readonly Action<object, object> _setPropertyValueFunc;
        private readonly Type _propertyType;

        public ExtensionPropertyDescriptor(
            string propertyName,
            Func<object, object> propertyValueFunc,
            Action<object, object> setPropertyValueFunc,
            Type propertyType,
            Attribute[] attributes) : base(propertyName, attributes)
        {
            _propertyValueFunc = propertyValueFunc;
            _setPropertyValueFunc = setPropertyValueFunc;
            _propertyType = propertyType;
        }

        public override void ResetValue(object component)
        {
        }

        public override bool CanResetValue(object component) => true;

        public override object GetValue(object component) => _propertyValueFunc(component);

        public override void SetValue(object component, object value) => _setPropertyValueFunc(component, value);

        public override bool ShouldSerializeValue(object component) => true;
        public override Type ComponentType => typeof(T);
        public override bool IsReadOnly => _setPropertyValueFunc == null;
        public override Type PropertyType => _propertyType;
    }
}