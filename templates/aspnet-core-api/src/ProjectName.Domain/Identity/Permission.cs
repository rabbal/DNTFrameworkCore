using DNTFrameworkCore.Domain;

namespace ProjectName.Domain.Identity
{
    /// <summary>
    /// Base class for TPH inheritance strategy
    /// </summary>
    public abstract class Permission : TrackableEntity<long>, IHasRowIntegrity, ICreationTracking, IModificationTracking
    {
        public const int NameLength = 128;

        /// <summary>
        ///     Unique Name of the Permission
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Indicate This Permission Is Granted With Role/User or Not
        /// </summary>
        public bool IsGranted { get; set; } = true;
    }
}