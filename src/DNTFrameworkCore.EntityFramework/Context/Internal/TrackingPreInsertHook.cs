using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Timing;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class TrackingPreInsertHook : PreInsertHook<IEntity>
    {
        private readonly IUserSession _session;
        private readonly IDateTime _dateTime;

        public TrackingPreInsertHook(IUserSession session, IDateTime dateTime)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        protected override void Hook(IEntity entity, HookEntityMetadata metadata)
        {
            if (entity is IHasCreationDateTime hasCreationDateTimeEntity)
                hasCreationDateTimeEntity.CreationDateTime = _dateTime.UtcNow;

            if (!(entity is ICreationTracking creationTrackingEntity)) return;

            creationTrackingEntity.CreatorUserId = _session.UserId;
            creationTrackingEntity.CreatorIp = _session.UserIP;
            creationTrackingEntity.CreatorBrowserName = _session.UserBrowserName;
        }
    }
}