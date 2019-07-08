namespace DNTFrameworkCore.Licensing
{
    public interface ILicensedProduct
    {
        string ProductVersion { get; }
        string SerialNumber { get; }
    }
}