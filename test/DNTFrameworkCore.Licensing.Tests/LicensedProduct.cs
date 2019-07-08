namespace DNTFrameworkCore.Licensing.Tests
{
    public class LicensedProduct : ILicensedProduct
    {
        public LicensedProduct(string productVersion, string serialNumber)
        {
            ProductVersion = productVersion;
            SerialNumber = serialNumber;
        }

        public string ProductVersion { get; }
        public string SerialNumber { get; }
    }
}