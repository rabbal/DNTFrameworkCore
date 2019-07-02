using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Domain
{
    public interface IAggregateRoot : IEntity
    {
        IReadOnlyCollection<IDomainEvent> Events { get; }
        void ClearEvents();
    }

    public abstract class AggregateRoot : AggregateRoot<int>
    {
    }

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IReadOnlyCollection<IDomainEvent> Events => _events.AsReadOnly();

        protected virtual void AddDomainEvent(IDomainEvent newEvent)
        {
            _events.Add(newEvent);
        }

        public virtual void ClearEvents()
        {
            _events.Clear();
        }
    }
}