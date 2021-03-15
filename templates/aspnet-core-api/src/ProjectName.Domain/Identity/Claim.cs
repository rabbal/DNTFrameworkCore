using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    public abstract class Claim : Entity<long>, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int TypeLength = 256;

        public string Type { get; set; }
        public string Value { get; set; }
    }
}