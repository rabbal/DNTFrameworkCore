namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public interface IFullTrackable : ITrackable, IDeletionTracking
    {
    }

    public interface IFullTrackable<TUser> : IFullTrackable, ITrackable<TUser>, IDeletionTracking<TUser>
        where TUser : Entity<long>
    {
    }
}