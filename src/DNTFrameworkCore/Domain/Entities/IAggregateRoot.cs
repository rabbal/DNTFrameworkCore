namespace DNTFrameworkCore.Domain.Entities
{
    public interface IAggregateRoot : IEntity, IHasRowVersion
    {
    }
}