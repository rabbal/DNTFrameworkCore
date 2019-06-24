using System;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.MultiTenancy;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Timing;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context
{
    internal class PreInsertCreationTrackingHook : PreInsertHook<ICreationTracking>
    {
        private readonly IUserSession _session;
        private readonly IDateTime _dateTime;

        public PreInsertCreationTrackingHook(IUserSession session, IDateTime dateTime)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        protected override void Hook(ICreationTracking entity, HookEntityMetadata metadata)
        {
            metadata.Entry.Property(EFCore.CreationDateTime).CurrentValue = _dateTime.UtcNow;
            metadata.Entry.Property(EFCore.CreatorBrowserName).CurrentValue = _session.UserBrowserName;
            metadata.Entry.Property(EFCore.CreatorIp).CurrentValue = _session.UserIP;
            metadata.Entry.Property(EFCore.CreatorUserId).CurrentValue = _session.UserId;
        }
    }

    internal class PreUpdateModificationTrackingHook : PreUpdateHook<IModificationTracking>
    {
        private readonly IUserSession _session;
        private readonly IDateTime _dateTime;

        public PreUpdateModificationTrackingHook(IUserSession session, IDateTime dateTime)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        protected override void Hook(IModificationTracking entity, HookEntityMetadata metadata)
        {
            metadata.Entry.Property(EFCore.ModificationDateTime).CurrentValue = _dateTime.UtcNow;
            metadata.Entry.Property(EFCore.ModifierBrowserName).CurrentValue = _session.UserBrowserName;
            metadata.Entry.Property(EFCore.ModifierIp).CurrentValue = _session.UserIP;
            metadata.Entry.Property(EFCore.ModifierUserId).CurrentValue = _session.UserId;
        }
    }

    internal class PreInsertTenantEntityHook : PreInsertHook<ITenantEntity>
    {
        private readonly ITenant _tenant;

        public PreInsertTenantEntityHook(ITenant tenant)
        {
            _tenant = tenant ?? throw new ArgumentNullException(nameof(tenant));
        }

        protected override void Hook(ITenantEntity entity, HookEntityMetadata metadata)
        {
            if (!_tenant.HasValue) return;

            metadata.Entry.Property(EFCore.TenantId).CurrentValue = _tenant.Value.Id;
        }
    }

    internal class PreInsertHasRowLevelSecurityHook : PreInsertHook<IHasRowLevelSecurity>
    {
        private readonly IUserSession _session;

        public PreInsertHasRowLevelSecurityHook(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void Hook(IHasRowLevelSecurity entity, HookEntityMetadata metadata)
        {
            metadata.Entry.Property(EFCore.UserId).CurrentValue = _session.UserId;
        }
    }

    internal class PreDeleteSoftDeleteEntityHook : PreDeleteHook<ISoftDeleteEntity>
    {
        protected override void Hook(ISoftDeleteEntity entity, HookEntityMetadata metadata)
        {
            metadata.Entry.State = EntityState.Modified;
            metadata.Entry.Property(EFCore.IsDeleted).CurrentValue = true;
        }
    }

    internal class PreUpdateRowVersionHook : PreUpdateHook<IHasRowVersion>
    {
        protected override void Hook(IHasRowVersion entity, HookEntityMetadata metadata)
        {
            metadata.Entry.Property(EFCore.RowVersion).OriginalValue =
                metadata.Entry.Property(EFCore.RowVersion).CurrentValue;
        }
    }
}