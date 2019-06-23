using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    /// <summary>
    ///     A strongly typed PreActionHook.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this hook must watch for.</typeparam>
    public abstract class PreActionHook<TEntity> : IPreActionHook
    {
        /// <summary>
        ///     Gets a value indicating whether the hook is only used after successful <typeparamref name="TEntity" /> validation.
        /// </summary>
        /// <value>
        ///     <c>true</c> if requires validation; otherwise, <c>false</c>.
        /// </value>
        public virtual bool RequiresValidation => false;

        /// <summary>
        ///     Entity States that this hook must be registered to listen for.
        /// </summary>
        public abstract EntityState HookState { get; }

        /// <summary>
        ///     Implements the interface.  This causes the hook to only run for objects that are assignable to TEntity.
        /// </summary>
        public void Hook(object entity, HookEntityMetadata metadata)
        {
            if (entity is TEntity typedEntity)
                Hook(typedEntity, metadata);
        }

        /// <summary>
        ///     The logic to perform per entity before the registered action gets performed.
        ///     This gets run once per entity that has been changed.
        /// </summary>
        /// <param name="entity">The entity that is processed by Entity Framework.</param>
        /// <param name="metadata">Metadata about the entity in the context of this hook - such as state.</param>
        protected abstract void Hook(TEntity entity, HookEntityMetadata metadata);
    }
}