using System;
using System.Collections.Generic;
using System.Reflection;

namespace DNTFrameworkCore.Domain.Entities
{
    public abstract class ValueObject<T> : IEquatable<T>
        where T : ValueObject<T>
    {
        public virtual bool Equals(T other)
        {
            if (other == null)
                return false;

            var t = GetType();
            var otherType = other.GetType();

            if (t != otherType)
                return false;

            var fields = t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);

            foreach (var field in fields)
            {
                var value1 = field.GetValue(other);
                var value2 = field.GetValue(this);

                if (value1 == null)
                {
                    if (value2 != null)
                        return false;
                }
                else if (!value1.Equals(value2))
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = obj as T;

            return Equals(other);
        }

        public override int GetHashCode()
        {
            var fields = GetFields();

            const int startValue = 17;
            const int multiplier = 59;

            var hashCode = startValue;

            foreach (var field in fields)
            {
                var value = field.GetValue(this);

                if (value != null)
                    hashCode = hashCode * multiplier + value.GetHashCode();
            }

            return hashCode;
        }

        private IEnumerable<FieldInfo> GetFields()
        {
            var t = GetType();

            var fields = new List<FieldInfo>();

            while (t != typeof(object))
            {
                fields.AddRange(t.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public));

                t = t.BaseType;
            }

            return fields;
        }

        public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
        {
            return !(left == right);
        }
    }

    //public abstract class ValueObject<T>
    //    where T : ValueObject<T>
    //{
    //    #region Public Methods
    //    public override bool Equals(object obj)
    //    {
    //        var valueObject = obj as T;

    //        return !ReferenceEquals(valueObject, null) && EqualsCore(valueObject);
    //    }

    //    public override int GetHashCode()
    //    {
    //        return GetHashCodeCore();
    //    }
    //    #endregion

    //    #region Protected Methods
    //    protected abstract int GetHashCodeCore();
    //    protected abstract bool EqualsCore(T valueObject);

    //    #endregion

    //    #region Operators

    //    public static bool operator ==(ValueObject<T> left, ValueObject<T> right)
    //    {
    //        return Equals(left, right);
    //    }

    //    public static bool operator !=(ValueObject<T> left, ValueObject<T> right)
    //    {
    //        return !(left == right);
    //    }

    //    #endregion
    //}
}