using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.Domain
{
    public interface IEntity
    {
        IReadOnlyCollection<IDomainEvent> DomainEvents { get; }
        void EmptyDomainEvents();
    }

    public abstract class Entity : Entity<int>
    {
    }

    public abstract class Entity<TKey> : IEntity
        where TKey : IEquatable<TKey>
    {
        private List<IDomainEvent> _domainEvents;
        private int? _hash;
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();
        public virtual TKey Id { get; protected set; }

        protected Entity()
        {
        }

        protected Entity(TKey id) : this()
        {
        }

        public virtual void EmptyDomainEvents() => _domainEvents?.Clear();

        protected void AddDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        protected void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents?.Remove(domainEvent);

        public override int GetHashCode()
        {
            if (IsTransient()) return base.GetHashCode();

            _hash ??= (GetRealType().ToString() + Id).GetHashCode() ^ 31;

            return _hash.Value;
        }

        public virtual bool IsTransient()
        {
            if (EqualityComparer<TKey>.Default.Equals(Id, default)) return true;

            //Workaround for EF Core since it sets int/long to min value when attaching to dbContext
            if (typeof(TKey) == typeof(int)) return Convert.ToInt32(Id) <= 0;

            if (typeof(TKey) == typeof(long)) return Convert.ToInt64(Id) <= 0;

            return false;
        }

        public override bool Equals(object obj)
        {
            if (obj is not Entity<TKey> instance) return false;

            if (ReferenceEquals(this, instance)) return true;

            if (GetRealType() != instance.GetRealType()) return false;

            if (IsTransient() || instance.IsTransient()) return false;

            return Id.Equals(instance.Id);
        }

        public override string ToString() => $"[{GetRealType().Name}: {Id}]";

        public static bool operator ==(Entity<TKey> left, Entity<TKey> right) => Equals(left, right);

        public static bool operator !=(Entity<TKey> left, Entity<TKey> right) => !(left == right);
        protected virtual Type GetRealType() => this.GetUnproxiedType();

        protected void ThrowDomainException(string message, string details = default)
        {
            throw new DomainException(message, details);
        }
    }
}