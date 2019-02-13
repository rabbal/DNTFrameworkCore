namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public interface IDeletionTracking : IHasDeletionDateTime
    {
        string DeleterIp { get; set; }
        string DeleterBrowserName { get; set; }
        long? DeleterUserId { get; set; }
    }

    public interface IDeletionTracking<TUser> : IDeletionTracking
        where TUser : Entity
    {
        TUser DeleterUser { get; set; }
    }
}