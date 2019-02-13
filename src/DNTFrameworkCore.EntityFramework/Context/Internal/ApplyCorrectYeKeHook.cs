using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.EntityFramework.Context.Internal
{
    internal class ApplyCorrectYeKeHookPreUpdateHook : PreUpdateHook<IEntity>
    {
        protected override void Hook(IEntity entity, HookEntityMetadata metadata)
        {
            entity.ApplyCorrectYeKeToProperties();
        }
    }

    internal class ApplyCorrectYeKeHookPreInsertHook : PreInsertHook<IEntity>
    {
        protected override void Hook(IEntity entity, HookEntityMetadata metadata)
        {
            entity.ApplyCorrectYeKeToProperties();
        }
    }
}