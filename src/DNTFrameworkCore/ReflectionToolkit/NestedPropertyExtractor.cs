using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Helpers;

namespace DNTFrameworkCore.ReflectionToolkit
{
    /// <summary>
    /// A helper class for dumping nested property values
    /// </summary>
    public class NestedPropertyExtractor
    {
        private readonly IList<Property> _result = new List<Property>();
        private int _index;

        /// <summary>
        /// Dumps Nested Property Values
        /// </summary>
        /// <param name="data">an instance object</param>
        /// <param name="parent">parent object's name</param>
        /// <param name="dumpLevel">how many levels should be searched</param>
        /// <returns>Nested Property Values List</returns>
        public IList<Property> ExtractPropertyValues(object data, string parent = "", int dumpLevel = 2)
        {
            if (data == null) return null;

            var propertyGetters = FastReflection.Instance.GetGetterDelegates(data.GetType());
            foreach (var propertyGetter in propertyGetters)
            {
                var dataValue = propertyGetter.GetterFunc(data);
                var name = $"{parent}{propertyGetter.Name}";
                if (dataValue == null)
                {
                    var nullDisplayText = propertyGetter.MemberInfo.GetNullDisplayTextAttribute();
                    _result.Add(new Property
                    {
                        PropertyName = name,
                        PropertyValue = nullDisplayText,
                        PropertyIndex = _index++,
                        PropertyType = propertyGetter.PropertyType
                    });
                }

                else if (propertyGetter.PropertyType.GetTypeInfo().IsEnum)
                {
                    var enumValue = ((Enum) dataValue).GetEnumStringValue();
                    _result.Add(new Property
                    {
                        PropertyName = name,
                        PropertyValue = enumValue,
                        PropertyIndex = _index++,
                        PropertyType = propertyGetter.PropertyType
                    });
                }
                else if (IsNestedProperty(propertyGetter.PropertyType))
                {
                    _result.Add(new Property
                    {
                        PropertyName = name,
                        PropertyValue = dataValue,
                        PropertyIndex = _index++,
                        PropertyType = propertyGetter.PropertyType
                    });

                    if (parent.Split('.').Length > dumpLevel)
                    {
                        continue;
                    }

                    ExtractPropertyValues(dataValue, $"{name}.", dumpLevel);
                }
                else
                {
                    _result.Add(new Property
                    {
                        PropertyName = name,
                        PropertyValue = dataValue,
                        PropertyIndex = _index++,
                        PropertyType = propertyGetter.PropertyType
                    });
                }
            }

            return _result;
        }

        private static bool IsNestedProperty(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            var assemblyFullName = typeInfo.Assembly.FullName;
            if (assemblyFullName.StartsWith("mscorlib", StringComparison.OrdinalIgnoreCase) ||
                assemblyFullName.StartsWith("System.Private.CoreLib", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            return
                (typeInfo.IsClass || typeInfo.IsInterface) &&
                !typeInfo.IsValueType &&
                !string.IsNullOrEmpty(type.Namespace) &&
                !type.Namespace.StartsWith("System.", StringComparison.OrdinalIgnoreCase);
        }
    }

    /// <summary>
    /// A class to hold data of the main table's cells
    /// </summary>    
    [DebuggerDisplay("{PropertyIndex} - {PropertyName} - {PropertyValue} - {FormattedValue}")]
    public class Property
    {
        /// <summary>
        /// Property index of the current cell
        /// </summary>
        public int PropertyIndex { set; get; }

        /// <summary>
        /// Property name of the current cell
        /// </summary>
        public string PropertyName { set; get; }

        /// <summary>
        /// Property value of the current cell
        /// </summary>
        public object PropertyValue { set; get; }

        /// <summary>
        /// Type of the property.
        /// </summary>
        public Type PropertyType { set; get; }

        /// <summary>
        /// Formatted Property value of the current cell
        /// </summary>
        public string FormattedValue { set; get; }

        /// <summary>
        /// Determines whether PropertyName of the this instance and another specified object have the same string value.             
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity (interface or class).</typeparam>
        /// <param name="expression">The expression returning the entity property, in the form x =&gt; x.Id</param>
        /// <returns>true or false</returns>
        public bool PropertyNameEquals<TEntity>(Expression<Func<TEntity, object>> expression)
        {
            return PropertyName.Equals(PropertyHelper.Name(expression));
        }
    }
}