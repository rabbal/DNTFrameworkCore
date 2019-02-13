namespace DNTFrameworkCore.Domain.Entities
{
    public interface IHaveRowVersion
    {
        byte[] RowVersion { get; set; }
    }
}