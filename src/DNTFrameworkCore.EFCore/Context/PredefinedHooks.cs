using System;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Tenancy;
using DNTFrameworkCore.Timing;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context
{
    internal class PreInsertCreationTrackingHook<TUserId> : PreInsertHook<ICreationTracking>
        where TUserId : IEquatable<TUserId>
    {
        private readonly IUserSession _session;
        private readonly IDateTime _dateTime;

        public PreInsertCreationTrackingHook(IUserSession session, IDateTime dateTime)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        protected override void Hook(ICreationTracking entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.CreationDateTime).CurrentValue = _dateTime.UtcNow;
            metadata.Entry.Property(EFCore.CreatorBrowserName).CurrentValue = _session.UserBrowserName;
            metadata.Entry.Property(EFCore.CreatorIp).CurrentValue = _session.UserIP;
            metadata.Entry.Property(EFCore.CreatorUserId).CurrentValue = _session.UserId.FromString<TUserId>();
        }
    }

    internal class PreUpdateModificationTrackingHook<TUserId> : PreUpdateHook<IModificationTracking>
        where TUserId : IEquatable<TUserId>
    {
        private readonly IUserSession _session;
        private readonly IDateTime _dateTime;

        public PreUpdateModificationTrackingHook(IUserSession session, IDateTime dateTime)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _dateTime = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        protected override void Hook(IModificationTracking entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.ModificationDateTime).CurrentValue = _dateTime.UtcNow;
            metadata.Entry.Property(EFCore.ModifierBrowserName).CurrentValue = _session.UserBrowserName;
            metadata.Entry.Property(EFCore.ModifierIp).CurrentValue = _session.UserIP;
            metadata.Entry.Property(EFCore.ModifierUserId).CurrentValue = _session.UserId.FromString<TUserId>();
        }
    }

    internal class PreInsertTenantEntityHook<TTenantId> : PreInsertHook<ITenantEntity>
        where TTenantId : IEquatable<TTenantId>
    {
        private readonly ITenantSession _context;

        public PreInsertTenantEntityHook(ITenantSession context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected override void Hook(ITenantEntity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.TenantId).CurrentValue = _context.Tenant.Id.FromString<TTenantId>();
        }
    }

    internal class PreInsertHasRowLevelSecurityHook<TUserId> : PreInsertHook<IHasRowLevelSecurity>
        where TUserId : IEquatable<TUserId>
    {
        private readonly IUserSession _session;

        public PreInsertHasRowLevelSecurityHook(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void Hook(IHasRowLevelSecurity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.UserId).CurrentValue = _session.UserId.FromString<TUserId>();
        }
    }

    internal class PreDeleteSoftDeleteEntityHook : PreDeleteHook<ISoftDeleteEntity>
    {
        protected override void Hook(ISoftDeleteEntity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.State = EntityState.Modified;
            metadata.Entry.Property(EFCore.IsDeleted).CurrentValue = true;
        }
    }

    internal class PreUpdateRowVersionHook : PreUpdateHook<IHasRowVersion>
    {
        protected override void Hook(IHasRowVersion entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.Version).OriginalValue =
                metadata.Entry.Property(EFCore.Version).CurrentValue;
        }
    }

    internal class PreInsertRowIntegrityHook : PreInsertHook<IHasRowIntegrity>
    {
        private readonly IRowIntegrityHashAlgorithm _algorithm;

        public PreInsertRowIntegrityHook(IRowIntegrityHashAlgorithm algorithm)
        {
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        protected override void Hook(IHasRowIntegrity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(nameof(entity.Hash)).CurrentValue = _algorithm.HashRow(metadata.Properties);
        }
    }

    internal class PreUpdateRowIntegrityHook : PreUpdateHook<IHasRowIntegrity>
    {
        private readonly IRowIntegrityHashAlgorithm _algorithm;

        public PreUpdateRowIntegrityHook(IRowIntegrityHashAlgorithm algorithm)
        {
            _algorithm = algorithm ?? throw new ArgumentNullException(nameof(algorithm));
        }

        protected override void Hook(IHasRowIntegrity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(nameof(entity.Hash)).CurrentValue = _algorithm.HashRow(metadata.Properties);
        }
    }
}