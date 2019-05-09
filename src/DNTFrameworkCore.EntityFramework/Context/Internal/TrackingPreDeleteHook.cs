using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Timing;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class TrackingPreDeleteHook : PreDeleteHook<IEntity>
    {
        private readonly IUserSession _session;
        private readonly IDateTime _dateTime;

        public TrackingPreDeleteHook(IUserSession session, IDateTime dateTime)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        protected override void Hook(IEntity entity, HookEntityMetadata metadata)
        {
            if (entity is IHasDeletionDateTime hasDeletionDateTimeEntity)
                hasDeletionDateTimeEntity.DeletionDateTime = _dateTime.UtcNow;

            if (!(entity is IDeletionTracking deletionTrackingEntity)) return;

            deletionTrackingEntity.DeleterUserId = _session.UserId;
            deletionTrackingEntity.DeleterIp = _session.UserIP;
            deletionTrackingEntity.DeleterBrowserName = _session.UserBrowserName;
        }
    }
}