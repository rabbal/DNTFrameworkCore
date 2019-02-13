namespace DNTFrameworkCore.Domain.Entities
{
    public interface IHaveRowLevelSecurity
    {
        long UserId { get; set; }
    }
}