namespace DNTFrameworkCore.Domain
{
    public interface IDomainEvent
    {
        int Version { get; }
    }

    public abstract class DomainEvent : IDomainEvent
    {
        public virtual int Version { get; } = default;
    }
}