namespace DNTFrameworkCore.Domain.Entities
{
    /// <summary>
    /// Interface implemented by entities that are change-tracked.
    /// </summary>
    public interface ITrackedEntity
    {
        /// <summary>
        /// Change-tracking state of an entity.
        /// </summary>
        TrackingState TrackingState { get; set; }

        //Todo: Guid TrackingId { get; set; }
    }
}