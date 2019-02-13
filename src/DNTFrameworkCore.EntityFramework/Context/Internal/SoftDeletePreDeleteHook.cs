using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class SoftDeletePreDeleteHook : PreDeleteHook<ISoftDeleteEntity>
    {
        protected override void Hook(ISoftDeleteEntity entity, HookEntityMetadata metadata)
        {
            entity.IsDeleted = true;
            metadata.State = EntityState.Modified;
        }
    }
}