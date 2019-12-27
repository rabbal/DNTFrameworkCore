namespace DNTFrameworkCore.Domain
{
    public interface IHasRowIntegrity
    {
        string Hash { get; set; }
    }
}