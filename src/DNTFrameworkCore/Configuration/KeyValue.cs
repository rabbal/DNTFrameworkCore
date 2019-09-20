using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Configuration
{
    public class KeyValue : Entity, IModificationTracking, ICreationTracking, IHasRowIntegrity
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}