namespace DNTFrameworkCore.Domain
{
    public interface IAggregateRoot : IEntity
    {
    }

    public interface IAggregateRootVersion
    {
        int Version { get; }
        void IncrementVersion();
    }
}