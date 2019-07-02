using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Configuration
{
    public class ConfigurationValue : Entity, IModificationTracking, ICreationTracking
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}