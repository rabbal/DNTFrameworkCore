using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Exceptions;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{

    public class TenantIdPreInsertHook : PreInsertHook<ITenantEntity>
    {
        private readonly IUnitOfWork _uow;

        public TenantIdPreInsertHook(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        protected override void Hook(ITenantEntity entity, HookEntityMetadata metadata)
        {
            if (!(entity is ITenantEntity tenantEntity)) return;

            if (tenantEntity.TenantId != default)
                throw new FrameworkException("Can not set TenantId to 0 for ITenantEntity!");

            tenantEntity.TenantId = _uow.TenantId;
        }
    }
}