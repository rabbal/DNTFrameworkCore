using System;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context.Hooks;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class RowVersionPreUpdateHook : PreUpdateHook<IHaveRowVersion>
    {
        private readonly IUnitOfWork _uow;

        public RowVersionPreUpdateHook(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        protected override void Hook(IHaveRowVersion entity, HookEntityMetadata metadata)
        {
            _uow.Entry(entity).OriginalValues[nameof(IHaveRowVersion.RowVersion)] = entity.RowVersion;
        }
    }
}