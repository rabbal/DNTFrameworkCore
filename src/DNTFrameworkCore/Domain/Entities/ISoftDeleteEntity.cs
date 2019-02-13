namespace DNTFrameworkCore.Domain.Entities
{
    public interface ISoftDeleteEntity
    {
        bool IsDeleted { get; set; }
    }
}