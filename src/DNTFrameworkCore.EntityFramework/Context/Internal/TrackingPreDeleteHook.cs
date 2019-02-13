using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class TrackingPreDeleteHook : PreDeleteHook<IEntity>
    {
        private readonly IUserSession _session;

        public TrackingPreDeleteHook(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void Hook(IEntity entity, HookEntityMetadata metadata)
        {
            if (entity is IHasDeletionDateTime hasDeletionDateTimeEntity)
                hasDeletionDateTimeEntity.DeletionDateTime = DateTimeOffset.UtcNow;

            if (!(entity is IDeletionTracking deletionTrackingEntity)) return;

            deletionTrackingEntity.DeleterUserId = _session.UserId;
            deletionTrackingEntity.DeleterIp = _session.UserIP;
            deletionTrackingEntity.DeleterBrowserName = _session.UserBrowserName;
        }
    }
}