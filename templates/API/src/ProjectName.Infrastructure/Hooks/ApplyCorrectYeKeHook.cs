using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Context.Hooks;
using ProjectName.Common.PersianToolkit;

namespace ProjectName.Infrastructure.Hooks
{
    public class PreInsertApplyCorrectYeKeHook : PreInsertHook<IEntity>
    {
        protected override void Hook(IEntity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            entity.ApplyCorrectYeKeToProperties();
        }

        public override string Name => "YeKe";
    }

    public class PreUpdateApplyCorrectYeKeHook : PreUpdateHook<IEntity>
    {
        protected override void Hook(IEntity entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            entity.ApplyCorrectYeKeToProperties();
        }

        public override string Name => "YeKe";
    }
}