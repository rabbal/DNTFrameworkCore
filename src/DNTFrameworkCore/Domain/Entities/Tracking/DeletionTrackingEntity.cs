using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public abstract class DeletionTrackingEntity : DeletionTrackingEntity<int>
    {
    }

    public abstract class DeletionTrackingEntity<TKey> : Entity<TKey>, IDeletionTracking where TKey : IEquatable<TKey>
    {
        public bool IsDeleted { get; set; }
        public DateTimeOffset? DeletionDateTime { get; set; }
        public string DeleterIp { get; set; }
        public string DeleterBrowserName { get; set; }
        public long? DeleterUserId { get; set; }
    }

    public abstract class DeletionTrackingEntity<TKey, TUser> : DeletionTrackingEntity<TKey>, IDeletionTracking<TUser>
        where TUser : Entity<long> where TKey : IEquatable<TKey>
    {
        public TUser DeleterUser { get; set; }
    }
}