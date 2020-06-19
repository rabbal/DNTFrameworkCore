using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Identity
{
    public abstract class Claim : TrackableEntity<long>, IRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxTypeLength = 256;

        public string Type { get; set; }
        public string Value { get; set; }
    }
}