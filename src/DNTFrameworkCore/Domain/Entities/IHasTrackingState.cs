namespace DNTFrameworkCore.Domain.Entities
{
    public interface IHasTrackingState
    {
        TrackingState TrackingState { get; set; }

        //Todo: Guid TrackingId { get; set; }
    }
}