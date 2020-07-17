using System;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Tenancy;
using DNTFrameworkCore.Timing;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context
{
    public static class HookNames
    {
        public const string CreationTracking = nameof(CreationTracking);
        public const string ModificationTracking = nameof(ModificationTracking);
        public const string RowLevelSecurity = nameof(RowLevelSecurity);
        public const string DeletedEntity = nameof(DeletedEntity);
        public const string RowVersion = nameof(RowVersion);
        public const string RowIntegrity = nameof(RowIntegrity);
        public const string Numbering = nameof(Numbering);
        public const string Tenancy = nameof(Tenancy);
        public const string DomainEvent = nameof(DomainEvent);
    }

    //Under development
    internal sealed class DomainEventHook : PostActionHook<IAggregateRoot>
    {
        private readonly IEventBus _bus;
        public override string Name => HookNames.DomainEvent;
        public override EntityState HookState => EntityState.Unchanged;

        public DomainEventHook(IEventBus bus)
        {
            _bus = bus;
        }

        protected override void Hook(IAggregateRoot entity, HookEntityMetadata metadata, IUnitOfWork context)
        {
            //todo: async issue
        }
    }

    internal sealed class PreInsertCreationTrackingHook<TUserId> : PreInsertHook<ICreationTracking>
        where TUserId : IEquatable<TUserId>
    {
        private readonly IUserSession _session;
        private readonly IClock _clock;

        public PreInsertCreationTrackingHook(IUserSession session, IClock clock)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public override string Name => HookNames.CreationTracking;

        protected override void Hook(ICreationTracking entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.CreatedDateTime).CurrentValue = _clock.Now;
            metadata.Entry.Property(EFCore.CreatedByBrowserName).CurrentValue = _session.UserBrowserName;
            metadata.Entry.Property(EFCore.CreatedByIP).CurrentValue = _session.UserIP;
            metadata.Entry.Property(EFCore.CreatedByUserId).CurrentValue = _session.UserId.To<TUserId>();
        }
    }

    internal sealed class PreUpdateModificationTrackingHook<TUserId> : PreUpdateHook<IModificationTracking>
        where TUserId : IEquatable<TUserId>
    {
        private readonly IUserSession _session;
        private readonly IClock _clock;

        public PreUpdateModificationTrackingHook(IUserSession session, IClock clock)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        }

        public override string Name => HookNames.ModificationTracking;

        protected override void Hook(IModificationTracking entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.ModifiedDateTime).CurrentValue = _clock.Now;
            metadata.Entry.Property(EFCore.ModifiedByBrowserName).CurrentValue = _session.UserBrowserName;
            metadata.Entry.Property(EFCore.ModifiedByIP).CurrentValue = _session.UserIP;
            metadata.Entry.Property(EFCore.ModifiedByUserId).CurrentValue = _session.UserId.To<TUserId>();
        }
    }

    internal sealed class PreInsertTenantEntityHook<TTenantId> : PreInsertHook<ITenantEntity>
        where TTenantId : IEquatable<TTenantId>
    {
        private readonly ITenantSession _session;

        public PreInsertTenantEntityHook(ITenantSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public override string Name => HookNames.Tenancy;

        protected override void Hook(ITenantEntity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.TenantId).CurrentValue = _session.TenantId.To<TTenantId>();
        }
    }

    internal sealed class PreInsertRowLevelSecurityHook<TUserId> : PreInsertHook<IHasRowLevelSecurity>
        where TUserId : IEquatable<TUserId>
    {
        private readonly IUserSession _session;

        public PreInsertRowLevelSecurityHook(IUserSession session)
        {
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        public override string Name => HookNames.RowLevelSecurity;

        protected override void Hook(IHasRowLevelSecurity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.UserId).CurrentValue = _session.UserId.To<TUserId>();
        }
    }

    internal sealed class PreDeleteDeletedEntityHook : PreDeleteHook<IDeletedEntity>
    {
        public override string Name => HookNames.DeletedEntity;

        protected override void Hook(IDeletedEntity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.State = EntityState.Modified;
            metadata.Entry.Property(EFCore.IsDeleted).CurrentValue = true;
        }
    }

    internal sealed class PreUpdateRowVersionHook : PreUpdateHook<IHasRowVersion>
    {
        public override string Name => HookNames.RowVersion;

        protected override void Hook(IHasRowVersion entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.Version).OriginalValue =
                metadata.Entry.Property(EFCore.Version).CurrentValue;
        }
    }

    internal sealed class RowIntegrityHook : PostActionHook<IHasRowIntegrity>
    {
        public override string Name => HookNames.RowIntegrity;
        public override int Order => int.MaxValue;
        public override EntityState HookState => EntityState.Unchanged;

        protected override void Hook(IHasRowIntegrity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            metadata.Entry.Property(EFCore.Hash).CurrentValue = uow.EntityHash(entity);
        }
    }
}