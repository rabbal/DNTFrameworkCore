using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Exceptions;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    public class BranchIdPreInsertHook : PreInsertHook<IBranchedEntity>
    {
        private readonly IUnitOfWork _uow;

        public BranchIdPreInsertHook(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        protected override void Hook(IBranchedEntity entity, HookEntityMetadata metadata)
        {
            if (!(entity is IBranchedEntity branchedEntity)) return;

            if (branchedEntity.BranchId != default)
                throw new FrameworkException("Can not set TenantId to 0 for IBranchedEntity!");

            branchedEntity.BranchId= _uow.BranchId;
        }
    }
}