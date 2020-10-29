using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Configuration
{
    public class KeyValue : Entity<long>, IHasRowVersion, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public virtual string Key { get; set; }
        public virtual string Value { get; set; }
        public byte[] Version { get; set; }
    }
}