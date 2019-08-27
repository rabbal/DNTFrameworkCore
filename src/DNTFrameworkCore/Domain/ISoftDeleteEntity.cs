namespace DNTFrameworkCore.Domain
{
    public interface ISoftDeleteEntity
    {
        bool IsDeleted { get; }
    }
}