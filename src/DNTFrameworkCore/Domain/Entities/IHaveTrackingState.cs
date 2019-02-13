namespace DNTFrameworkCore.Domain.Entities
{
    /// <summary>
    /// Interface implemented by entities that are change-tracked.
    /// </summary>
    public interface IHaveTrackingState
    {
        /// <summary>
        /// Change-tracking state of an entity.
        /// </summary>
        TrackingState TrackingState { get; set; }
    }
}