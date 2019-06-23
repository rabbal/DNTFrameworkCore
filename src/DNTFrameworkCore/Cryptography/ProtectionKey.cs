using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Cryptography
{
    public class ProtectionKey : Entity<long>
    {
        public string FriendlyName { get; set; }
        public string XmlValue { get; set; }
    }
}