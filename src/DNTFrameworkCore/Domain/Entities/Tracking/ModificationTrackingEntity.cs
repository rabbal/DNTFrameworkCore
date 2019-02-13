using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public abstract class ModificationTrackingEntity : ModificationTrackingEntity<long>
    {
    }

    public abstract class ModificationTrackingEntity<TKey> : Entity<TKey>, IModificationTracking
        where TKey : IEquatable<TKey>
    {
        public DateTimeOffset? LastModificationDateTime { get; set; }
        public string LastModifierIp { get; set; }
        public string LastModifierBrowserName { get; set; }
        public long? LastModifierUserId { get; set; }
    }

    public abstract class ModificationTrackingEntity<TKey, TUser> : ModificationTrackingEntity<TKey>,
        IModificationTracking<TUser>
        where TUser : Entity where TKey : IEquatable<TKey>
    {
        public TUser LastModifierUser { get; set; }
    }
}