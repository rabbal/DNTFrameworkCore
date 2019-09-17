using DNTFrameworkCore.Timing;
using DateTime = System.DateTime;

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
        public virtual DateTime DateTime => SystemTime.UtcNow.DateTime;
    }
}