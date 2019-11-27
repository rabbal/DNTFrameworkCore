using DNTFrameworkCore.Timing;
using System;

namespace DNTFrameworkCore.Domain
{
    public interface IDomainEvent
    {
        int Version { get; }
        DateTime DateTime { get; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public virtual int Version { get; } = default;
        public virtual DateTime DateTime => SystemTime.UtcNow;
    }
}