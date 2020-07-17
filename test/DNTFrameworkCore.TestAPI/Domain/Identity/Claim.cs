using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public abstract class Claim : TrackableEntity, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxTypeLength = 256;

        public string Type { get; set; }
        public string Value { get; set; }
    }
}