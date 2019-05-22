namespace DNTFrameworkCore.Domain.Entities
{
    public interface IHasRowLevelSecurity
    {
        long UserId { get; set; }
    }
}