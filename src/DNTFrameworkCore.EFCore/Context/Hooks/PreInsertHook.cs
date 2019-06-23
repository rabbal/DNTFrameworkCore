using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    /// <summary>
    /// Implements a hook that will run before an entity gets inserted into the database.
    /// </summary>
    public abstract class PreInsertHook<TEntity> : PreActionHook<TEntity>
    {
        /// <summary>
        /// Returns <see cref="EntityState.Added"/> as the hookState to listen for.
        /// </summary>
        public override EntityState HookState => EntityState.Added;
    }
}