namespace DNTFrameworkCore.Domain
{
    public interface IHasRowVersion
    {
        byte[] Version { get; set; }
    }
}