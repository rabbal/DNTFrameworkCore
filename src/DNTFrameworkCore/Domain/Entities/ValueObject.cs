using System;
using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Domain.Entities
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> EqualityValues { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (GetType() != obj.GetType()) return false;

            var valueObject = (ValueObject)obj;

            return EqualityValues.SequenceEqual(valueObject.EqualityValues);
        }

        public override int GetHashCode()
        {
            return EqualityValues
                .Aggregate(1, (current, obj) =>
                {
                    unchecked
                    {
                        return current * 23 + (obj?.GetHashCode() ?? 0);
                    }
                });
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (ReferenceEquals(left, null) && ReferenceEquals(right, null))
                return true;

            if (ReferenceEquals(left, null) || ReferenceEquals(right, null))
                return false;

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }

        public ValueObject Copy()
        {
            return MemberwiseClone() as ValueObject;
        }
    }
}