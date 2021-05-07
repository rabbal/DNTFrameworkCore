using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Domain
{
    public abstract class ValueObject
    {
        protected abstract IEnumerable<object> EqualityValues { get; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            if (GetType() != obj.GetType()) return false;

            var valueObject = (ValueObject) obj;

            return EqualityValues.SequenceEqual(valueObject.EqualityValues);
        }

        public override int GetHashCode()
        {
            return EqualityValues
                .Aggregate(17, (current, next) =>
                {
                    unchecked
                    {
                        return current * 23 + (next?.GetHashCode() ?? 0);
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

        protected void ThrowDomainException(string message, string details = default)
        {
            throw new DomainException(message, details);
        }
    }
}