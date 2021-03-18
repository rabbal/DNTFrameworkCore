using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Identity
{
    public abstract class Claim : Entity<long>, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxTypeLength = 256;

        public string Type { get; set; }
        public string Value { get; set; }
    }
}