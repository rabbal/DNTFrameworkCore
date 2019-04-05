namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public interface ICreationTracking : IHasCreationDateTime
    {
        string CreatorIp { get; set; }
        string CreatorBrowserName { get; set; }
        long? CreatorUserId { get; set; }
    }

    public interface ICreationTracking<TUser> : ICreationTracking
        where TUser : Entity<long>
    {
        TUser CreatorUser { get; set; }
    }
}