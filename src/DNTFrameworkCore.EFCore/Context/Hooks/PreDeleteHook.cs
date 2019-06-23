using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    /// <summary>
    /// Implements a hook that will run before an entity gets deleted from the database.
    /// </summary>
    public abstract class PreDeleteHook<TEntity> : PreActionHook<TEntity>
    {
        /// <summary>
        /// Returns <see cref="EntityState.Deleted"/> as the hookState to listen for.
        /// </summary>
        public override EntityState HookState => EntityState.Deleted;
    }
}