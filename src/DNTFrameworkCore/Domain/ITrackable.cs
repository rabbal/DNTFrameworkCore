using System.Collections.Generic;

namespace DNTFrameworkCore.Domain
{
    /// <summary>
    /// Interface implemented by entities that are change-tracked.
    /// </summary>
    public interface ITrackable
    {
        /// <summary>
        /// Change-tracking state of an entity.
        /// </summary>
        TrackingState TrackingState { get; set; }

        /// <summary>
        /// Properties on an entity that have been modified.
        /// </summary>
        ICollection<string> ModifiedProperties { get; set; }
    }

    /// <summary>
    /// Change-tracking state of an entity.
    /// </summary>
    public enum TrackingState
    {
        Unchanged,
        Added,
        Modified,
        Deleted,
    }
}