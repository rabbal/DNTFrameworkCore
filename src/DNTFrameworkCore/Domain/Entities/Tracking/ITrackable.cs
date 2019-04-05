namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public interface ITrackable : ICreationTracking, IModificationTracking
    {
    }

    public interface ITrackable<TUser> : ITrackable, ICreationTracking<TUser>, IModificationTracking<TUser>
        where TUser : Entity<long>
    {
    }
}