using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Domain
{
    public interface IAggregateRoot : IEntity
    {
        IEnumerable<IDomainEvent> Events { get; }
        void EmptyEvents();
    }

    public abstract class AggregateRoot : AggregateRoot<int>
    {
    }

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IEnumerable<IDomainEvent> Events => _events.AsReadOnly();
        public virtual void EmptyEvents() => _events.Clear();
        protected virtual void RaiseDomainEvent(IDomainEvent domainEvent) => _events.Add(domainEvent);
    }

    public interface IAggregateRootRowVersion
    {
        int Version { get; }
        void IncreaseVersion();
    }
}