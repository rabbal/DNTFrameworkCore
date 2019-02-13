using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public abstract class CreationTrackingEntity : CreationTrackingEntity<long>
    {
    }

    public abstract class CreationTrackingEntity<TKey> : Entity<TKey>, ICreationTracking where TKey : IEquatable<TKey>
    {
        public DateTimeOffset CreationDateTime { get; set; }
        public string CreatorBrowserName { get; set; }
        public string CreatorIp { get; set; }
        public long? CreatorUserId { get; set; }
    }

    public abstract class CreationTrackingEntity<TKey, TUser> : CreationTrackingEntity<TKey>, ICreationTracking<TUser>
        where TUser : Entity where TKey : IEquatable<TKey>
    {
        public TUser CreatorUser { get; set; }
    }
}