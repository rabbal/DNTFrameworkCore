using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.EntityFramework.Protection
{
    public class ProtectionKey : Entity<long>
    {
        public string FriendlyName { get; set; }
        public string XmlValue { get; set; }
    }
}