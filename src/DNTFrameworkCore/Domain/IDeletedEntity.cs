namespace DNTFrameworkCore.Domain
{
    public interface IDeletedEntity
    {
        bool IsDeleted { get; }
    }
}