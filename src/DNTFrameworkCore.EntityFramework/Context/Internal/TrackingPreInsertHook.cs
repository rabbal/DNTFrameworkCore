using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class TrackingPreInsertHook : PreInsertHook<IEntity>
    {
        private readonly IUserSession _session;

        public TrackingPreInsertHook(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void Hook(IEntity entity, HookEntityMetadata metadata)
        {
            if (entity is IHasCreationDateTime hasCreationDateTimeEntity)
                hasCreationDateTimeEntity.CreationDateTime = DateTimeOffset.UtcNow;

            if (!(entity is ICreationTracking creationTrackingEntity)) return;
            
            creationTrackingEntity.CreatorUserId = _session.UserId;
            creationTrackingEntity.CreatorIp = _session.UserIP;
            creationTrackingEntity.CreatorBrowserName = _session.UserBrowserName;
        }
    }
}