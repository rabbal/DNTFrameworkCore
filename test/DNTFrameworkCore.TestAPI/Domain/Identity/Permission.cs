using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestAPI.Domain.Identity
{
    public abstract class Permission : TrackableEntity, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int MaxNameLength = 128;
        public string Name { get; set; }
        public bool IsGranted { get; set; } = true;
    }
}