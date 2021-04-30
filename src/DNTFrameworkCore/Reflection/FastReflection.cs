using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using DNTFrameworkCore.Common;

namespace DNTFrameworkCore.Reflection
{
    /// <summary>
    /// Fast property access, using Reflection.Emit.
    /// </summary>
    public class FastReflection
    {
        private static readonly Lazy<FastReflection> _instance =
            new(() => new FastReflection(), LazyThreadSafetyMode.ExecutionAndPublication);

        private readonly LockingConcurrentDictionary<Type, FastPropertyInfo[]> _properties = new();
        private readonly LockingConcurrentDictionary<Type, FastFieldInfo[]> _fields = new();
        private readonly LockingConcurrentDictionary<PropertyInfo, FastPropertyInfo> _propertyStore = new();

        private FastReflection()
        {
        }

        public static FastReflection Instance => _instance.Value;

        /// <summary>
        /// Fast property access, using Reflection.Emit.
        /// </summary>
        public FastPropertyInfo[] GetProperties(Type type)
        {
            return _properties.GetOrAdd(type, static type =>
            {
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                return properties.Select(FastPropertyInfo.From).ToArray();
            });
        }

        /// <summary>
        /// Fast field access, using Reflection.Emit.
        /// </summary>
        public FastFieldInfo[] GetFields(Type type)
        {
            return _fields.GetOrAdd(type, static type =>
            {
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                return fields.Select(FastFieldInfo.From).ToArray();
            });
        }

        public FastPropertyInfo GetProperty(PropertyInfo property)
        {
            return _propertyStore.GetOrAdd(property, FastPropertyInfo.From);
        }
    }

    public class FastPropertyInfo
    {
        public Func<object, object> GetValue { get; private set; }
        public Action<object, object> SetValue { get; private set; }

        /// <summary>
        /// Obtains information about the attributes of a member and provides access.
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        /// <summary>
        /// Property's name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Property's Type.
        /// </summary>
        public Type PropertyType { get; private set; }

        public bool CanRead => GetValue != null;
        public bool CanWrite => SetValue != null;

        private FastPropertyInfo()
        {
        }

        public static FastPropertyInfo From(PropertyInfo property)
        {
            return new()
            {
                Name = property.Name,
                GetValue = CreateGetterPropertyDelegate(property.DeclaringType, property),
                SetValue = CreateSetterPropertyDelegate(property.DeclaringType, property),
                PropertyType = property.PropertyType,
                MemberInfo = property
            };
        }

        private static Func<object, object> CreateGetterPropertyDelegate(Type type, PropertyInfo propertyInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var getterExpression =
                Expression.Convert(Expression.Property(Expression.Convert(instanceExpression, type), propertyInfo),
                    typeof(object));
            return Expression.Lambda<Func<object, object>>(getterExpression, instanceExpression).Compile();
        }

        private static Action<object, object> CreateSetterPropertyDelegate(Type type, PropertyInfo propertyInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var paramExpression = Expression.Parameter(typeof(object), "param");

            var setterExpression =
                Expression.Assign(Expression.Property(Expression.Convert(instanceExpression, type), propertyInfo),
                    Expression.Convert(paramExpression, propertyInfo.PropertyType));

            return Expression.Lambda<Action<object, object>>(setterExpression, instanceExpression, paramExpression)
                .Compile();
        }
    }

    public class FastFieldInfo
    {
        public Func<object, object> GetValue { get; private set; }
        public Action<object, object> SetValue { get; private set; }

        /// <summary>
        /// Obtains information about the attributes of a member and provides access.
        /// </summary>
        public MemberInfo MemberInfo { get; private set; }

        public string Name { get; private set; }
        public Type FieldType { get; private set; }

        private FastFieldInfo()
        {
        }

        public static FastFieldInfo From(FieldInfo field)
        {
            return new()
            {
                Name = field.Name,
                GetValue = CreateGetterFieldDelegate(field.DeclaringType, field),
                SetValue = CreateSetterFieldDelegate(field.DeclaringType, field),
                FieldType = field.FieldType,
                MemberInfo = field
            };
        }

        private static Func<object, object> CreateGetterFieldDelegate(Type type, FieldInfo fieldInfo)
        {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var field = Expression.Field(Expression.TypeAs(instanceParam, type), fieldInfo);
            var convertField = Expression.TypeAs(field, typeof(object));
            return Expression.Lambda<Func<object, object>>(convertField, instanceParam).Compile();
        }

        private static Action<object, object> CreateSetterFieldDelegate(Type type, FieldInfo fieldInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var paramExpression = Expression.Parameter(typeof(object), "param");

            var assignExpression =
                Expression.Assign(Expression.Field(Expression.TypeAs(instanceExpression, type), fieldInfo),
                    Expression.TypeAs(paramExpression, typeof(object)));
            return Expression.Lambda<Action<object, object>>(Expression.TypeAs(assignExpression, typeof(object)),
                    instanceExpression, paramExpression)
                .Compile();
        }
    }
}