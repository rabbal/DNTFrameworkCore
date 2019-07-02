using System.Collections.Generic;
using System.Linq;
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

        protected static Result Ok() => Result.Ok();
        protected static Result Fail(string message) => Result.Fail(message);

        protected static Result Fail(string message, IEnumerable<ValidationFailure> failures) =>
            Result.Fail(message, failures);

        protected static Result<T> Ok<T>(T value) => Result.Ok(value);
        protected static Result<T> Fail<T>(string message) => Result.Fail<T>(message);

        protected static Result<T> Fail<T>(string message, IEnumerable<ValidationFailure> failures) =>
            Result.Fail<T>(message, failures);
    }
}