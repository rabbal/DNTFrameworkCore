using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.Domain
{
    public interface IAggregateRoot : IEntity
    {
        IReadOnlyList<IDomainEvent> Events { get; }
        int VersionId { get; }
        void IncreaseVersion();
    }

    public abstract class AggregateRoot : AggregateRoot<int>
    {
    }

    public abstract class AggregateRoot<TKey> : Entity<TKey>, IAggregateRoot
        where TKey : IEquatable<TKey>
    {
        private readonly List<IDomainEvent> _events = new List<IDomainEvent>();
        public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();
        public virtual int VersionId { get; protected set; }

        protected virtual void AddDomainEvent(IDomainEvent newEvent)
        {
            _events.Add(newEvent);
        }

        public void IncreaseVersion()
        {
            VersionId++;
        }
    }
}