using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public abstract class TrackableEntity : TrackableEntity<int>
    {
    }

    public abstract class TrackableEntity<TKey> : Entity<TKey>, ITrackable where TKey : IEquatable<TKey>
    {
        public DateTimeOffset CreationDateTime { get; set; }
        public DateTimeOffset? LastModificationDateTime { get; set; }
        public string CreatorIp { get; set; }
        public string LastModifierIp { get; set; }
        public string CreatorBrowserName { get; set; }
        public string LastModifierBrowserName { get; set; }
        public long? LastModifierUserId { get; set; }
        public long? CreatorUserId { get; set; }
    }

    public abstract class TrackableEntity<TKey, TUser> : TrackableEntity<TKey>, ITrackable<TUser>
        where TUser : Entity<long> where TKey : IEquatable<TKey>
    {
        public TUser CreatorUser { get; set; }
        public TUser LastModifierUser { get; set; }
    }
}