using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.EntityFramework.Configuration
{
    public class ConfigurationValue : Entity<string>
    {
        public string Value { get; set; }
    }
}
