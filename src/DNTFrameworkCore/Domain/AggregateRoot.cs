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
        private List<IDomainEvent> _domainEvents;
        public IEnumerable<IDomainEvent> DomainEvents => _domainEvents?.AsReadOnly();
        public virtual void EmptyDomainEvents() => _domainEvents?.Clear();

        protected virtual void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents ??= new List<IDomainEvent>();
            _domainEvents.Add(domainEvent);
        }

        protected virtual void RemoveDomainEvent(IDomainEvent domainEvent) => _domainEvents?.Remove(domainEvent);
    }

    public interface IAggregateRootRowVersion
    {
        int Version { get; }
        void IncreaseVersion();
    }
}