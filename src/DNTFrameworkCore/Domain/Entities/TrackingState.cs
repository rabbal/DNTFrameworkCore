namespace DNTFrameworkCore.Domain.Entities
{
    /// <summary>
    /// Change-tracking state of an entity.
    /// </summary>
    public enum TrackingState
    {
        /// <summary>Existing entity that has not been modified.</summary>
        Unchanged = 0,

        /// <summary>Newly created entity.</summary>
        Added = 1,

        /// <summary>Existing entity that has been modified.</summary>
        Modified = 2,

        /// <summary>Existing entity that has been marked as deleted.</summary>
        Deleted = 3
    }
}