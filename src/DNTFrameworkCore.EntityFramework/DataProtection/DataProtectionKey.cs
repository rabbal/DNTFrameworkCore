using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.EntityFramework.DataProtection
{
    public class DataProtectionKey : Entity<long>
    {
        public string FriendlyName { get; set; }
        public string XmlValue { get; set; }
    }
}