namespace DNTFrameworkCore.Licensing.Tests
{
    public class LicensedProduct : ILicensedProduct
    {
        public LicensedProduct(string productVersion, string serialNumber, string productName)
        {
            ProductVersion = productVersion;
            UniqueId = serialNumber;
            ProductName = productName;
        }

        public string ProductName { get; }
        public string ProductVersion { get; }
        public string UniqueId { get; }
    }
}