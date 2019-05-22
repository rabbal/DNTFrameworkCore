namespace DNTFrameworkCore.Domain.Entities
{
    public interface IHasRowVersion
    {
        byte[] RowVersion { get; set; }
    }
}