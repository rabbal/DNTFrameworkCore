namespace DNTFrameworkCore.Domain.Entities
{
    public interface IPassivableEntity
    {
        bool IsActive { get; set; }
    }
}