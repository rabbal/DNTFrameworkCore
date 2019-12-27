namespace DNTFrameworkCore.TestTenancy.Domain.Identity
{
    /// <summary>
    /// Base Class For TPH Inheritance Strategy
    /// Storage of System's Permissions
    /// </summary>
    public abstract class Permission : TrackableEntity, IModificationTracking
    {
        public const int MaxNameLength = 128;

        /// <summary>
        /// Unique Name of the Permission
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Indicate This Permission Is Granted With Role/User or Not
        /// </summary>
        public bool IsGranted { get; set; } = true;
    }
}