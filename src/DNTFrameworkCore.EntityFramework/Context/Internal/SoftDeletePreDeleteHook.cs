using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Timing;
using Microsoft.EntityFrameworkCore;
using System;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class SoftDeletePreDeleteHook : PreDeleteHook<ISoftDeleteEntity>
    {
        private readonly IUserSession _session;
        private readonly IDateTime _dateTime;

        public SoftDeletePreDeleteHook(IUserSession session, IDateTime dateTime)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        protected override void Hook(ISoftDeleteEntity entity, HookEntityMetadata metadata)
        {
            entity.IsDeleted = true;
            metadata.State = EntityState.Modified;

            if (entity is IHasModificationDateTime hasModificationDateTimeEntity)
            {
                hasModificationDateTimeEntity.LastModificationDateTime = _dateTime.UtcNow;
            }

            if (!(entity is IModificationTracking modificationTrackingEntity)) return;

            modificationTrackingEntity.LastModifierUserId = _session.UserId;
            modificationTrackingEntity.LastModifierIp = _session.UserIP;
            modificationTrackingEntity.LastModifierBrowserName = _session.UserBrowserName;
        }
    }
}