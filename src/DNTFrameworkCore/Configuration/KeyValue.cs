using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Configuration
{
    public class KeyValue : Entity<long>, IRowVersion, IRowIntegrity, ICreationTracking, IModificationTracking
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
    }
}