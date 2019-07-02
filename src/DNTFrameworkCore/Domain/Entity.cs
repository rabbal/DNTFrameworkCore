using System;
using System.Collections.Generic;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Domain
{
    public interface IEntity
    {
    }

    public abstract class Entity : Entity<int>
    {
    }

    public abstract class Entity<TKey> : IEntity
        where TKey : IEquatable<TKey>
    {
        private int? _hashCode;
        public virtual TKey Id { get; protected set; }

        public override int GetHashCode()
        {
            if (IsTransient()) return base.GetHashCode();

            if (!_hashCode.HasValue)
                _hashCode = (GetRealType().ToString() + Id).GetHashCode() ^ 31; // XOR for random distribution

            return _hashCode.Value;
        }

        public bool IsTransient()
        {
            if (EqualityComparer<TKey>.Default.Equals(Id, default)) return true;

            //Workaround for EF Core since it sets int/long to min value when attaching to dbContext
            if (typeof(TKey) == typeof(int)) return Convert.ToInt32(Id) <= 0;

            if (typeof(TKey) == typeof(long)) return Convert.ToInt64(Id) <= 0;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is Entity<TKey> other)) return false;

            if (ReferenceEquals(this, other)) return true;

            if (GetRealType() != other.GetRealType()) return false;

            if (IsTransient() || other.IsTransient()) return false;

            return Id.Equals(other.Id);
        }

        public override string ToString()
        {
            return $"[{GetRealType().Name} : {Id}]";
        }

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right)
        {
            return !(left == right);
        }

        protected virtual Type GetRealType()
        {
            return GetType();
        }

        protected static Result Ok()
        {
            return Result.Ok();
        }

        protected static Result Fail(string message)
        {
            return Result.Fail(message);
        }

        protected static Result Fail(string message, IEnumerable<ValidationFailure> failures)
        {
            return Result.Fail(message, failures);
        }

        protected static Result<T> Ok<T>(T value)
        {
            return Result.Ok(value);
        }

        protected static Result<T> Fail<T>(string message)
        {
            return Result.Fail<T>(message);
        }

        protected static Result<T> Fail<T>(string message, IEnumerable<ValidationFailure> failures)
        {
            return Result.Fail<T>(message, failures);
        }
    }
}