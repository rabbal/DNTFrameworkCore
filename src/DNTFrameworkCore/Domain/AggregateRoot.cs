using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Domain
{
    public interface IAggregateRoot : IEntity
    {
        IEnumerable<IDomainEvent> DomainEvents { get; }
        void EmptyDomainEvents();
    }

    public abstract class AggregateRoot : AggregateRoot<int>
    {
    }

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        private readonly List<IDomainEvent> _events = new();
        public IEnumerable<IDomainEvent> DomainEvents => _events.AsReadOnly();
        public virtual void EmptyDomainEvents() => _events.Clear();
        protected virtual void RaiseDomainEvent(IDomainEvent domainEvent) => _events.Add(domainEvent);
        protected virtual void RemoveDomainEvent(IDomainEvent domainEvent) => _events.Remove(domainEvent);
    }

    public interface IAggregateRootRowVersion
    {
        int Version { get; }
        void IncreaseVersion();
    }
}