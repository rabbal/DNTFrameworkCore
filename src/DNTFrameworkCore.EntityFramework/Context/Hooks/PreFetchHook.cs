using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context.Hooks
{
    public abstract class PreFetchHook<TEntity> : PreActionHook<TEntity>
    {
        public override EntityState HookState => EntityState.Unchanged;
    }
}