using System;

namespace DNTFrameworkCore.Domain.Entities.Tracking
{
    public abstract class FullTrackableEntity : FullTrackableEntity<int>
    {
    }

    public abstract class FullTrackableEntity<TKey> : Entity<TKey>, IFullTrackable where TKey : IEquatable<TKey>
    {
        public DateTimeOffset CreationDateTime { get; set; }
        public DateTimeOffset? LastModificationDateTime { get; set; }
        public DateTimeOffset? DeletionDateTime { get; set; }
        public string CreatorIp { get; set; }
        public string LastModifierIp { get; set; }
        public string DeleterIp { get; set; }
        public string CreatorBrowserName { get; set; }
        public string LastModifierBrowserName { get; set; }
        public string DeleterBrowserName { get; set; }
        public long? LastModifierUserId { get; set; }
        public long? CreatorUserId { get; set; }
        public long? DeleterUserId { get; set; }
        public bool IsDeleted { get; set; }
    }

    public abstract class FullTrackableEntity<TKey, TUser> : FullTrackableEntity<TKey>, IFullTrackable<TUser>
        where TUser : Entity where TKey : IEquatable<TKey>
    {
        public TUser CreatorUser { get; set; }
        public TUser LastModifierUser { get; set; }
        public TUser DeleterUser { get; set; }
    }
}