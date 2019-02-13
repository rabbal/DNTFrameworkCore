using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class TrackingPreUpdateHook : PreUpdateHook<IEntity>
    {
        private readonly IUserSession _session;

        public TrackingPreUpdateHook(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void Hook(IEntity entity, HookEntityMetadata metadata)
        {
            if (entity is IHasModificationDateTime hasModificationDateTimeEntity)
            {
                hasModificationDateTimeEntity.LastModificationDateTime = DateTimeOffset.UtcNow;
            }

            if (!(entity is IModificationTracking modificationTrackingEntity)) return;
            
            modificationTrackingEntity.LastModifierUserId = _session.UserId;
            modificationTrackingEntity.LastModifierIp = _session.UserIP;
            modificationTrackingEntity.LastModifierBrowserName = _session.UserBrowserName;
        }
    }
}