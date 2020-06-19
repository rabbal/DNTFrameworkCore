namespace DNTFrameworkCore.Licensing
{
    public interface ILicensedProduct
    {
        string ProductName { get; }
        string ProductVersion { get; }
        string SecurityStamp { get; }
    }
}