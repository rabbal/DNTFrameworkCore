using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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

        private readonly LockingConcurrentDictionary<Type, List<FastPropertyInfo>> _properties = new();

        //TODO: implement GetGetter method
        private readonly ConcurrentDictionary<PropertyInfo, Lazy<FastPropertyInfo>> _getters = new();
        private readonly ConcurrentDictionary<PropertyInfo, Lazy<FastPropertyInfo>> _setters = new();

        private FastReflection()
        {
        }

        public static FastReflection Instance { get; } = _instance.Value;

        /// <summary>
        /// Fast property access, using Reflection.Emit.
        /// </summary>
        public IEnumerable<FastPropertyInfo> GetProperties(Type type)
        {
            var getterDelegates = _properties.GetOrAdd(type, _ =>
            {
                var gettersList = new List<FastPropertyInfo>();
                var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                foreach (var property in properties)
                {
                    var info = new FastPropertyInfo
                    {
                        Name = property.Name,
                        GetValue = CreateGetterPropertyDelegate(type, property),
                        SetValue = CreateSetterPropertyDelegate(type, property),
                        PropertyType = property.PropertyType,
                        MemberInfo = property
                    };
                    gettersList.Add(info);
                }

                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public);
                foreach (var field in fields)
                {
                    var info = new FastPropertyInfo
                    {
                        Name = field.Name,
                        GetValue = CreateGetterFieldDelegate(type, field),
                        SetValue = CreateSetterFieldDelegate(type, field),
                        PropertyType = field.FieldType,
                        MemberInfo = field
                    };
                    gettersList.Add(info);
                }

                return gettersList;
            });
            return getterDelegates;
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

        private static Func<object, object> CreateGetterFieldDelegate(Type type, FieldInfo fieldInfo)
        {
            var instanceParam = Expression.Parameter(typeof(object), "instance");
            var field = Expression.Field(Expression.TypeAs(instanceParam, type), fieldInfo);
            var convertField = Expression.TypeAs(field, typeof(object));
            return Expression.Lambda<Func<object, object>>(convertField, instanceParam).Compile();
        }

        private static Func<object, object> CreateGetterPropertyDelegate(Type type, PropertyInfo propertyInfo)
        {
            var instanceExpression = Expression.Parameter(typeof(object), "instance");
            var getterExpression =
                Expression.Convert(Expression.Property(Expression.Convert(instanceExpression, type), propertyInfo),
                    typeof(object));
            return Expression.Lambda<Func<object, object>>(getterExpression, instanceExpression).Compile();
        }
    }

    public class FastPropertyInfo
    {
        public Func<object, object> GetValue { set; get; }
        public Action<object, object> SetValue { set; get; }

        /// <summary>
        /// Obtains information about the attributes of a member and provides access.
        /// </summary>
        public MemberInfo MemberInfo { set; get; }

        /// <summary>
        /// Property/Field's name.
        /// </summary>
        public string Name { set; get; }

        /// <summary>
        /// Property/Field's Type.
        /// </summary>
        public Type PropertyType { set; get; }

        public bool CanRead => GetValue != null;
        public bool CanWrite => SetValue != null;
    }
}