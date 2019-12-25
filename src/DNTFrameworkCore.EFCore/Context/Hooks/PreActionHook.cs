using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    /// <summary>
    /// A strongly typed PreActionHook.
    /// </summary>
    /// <typeparam name="TEntity">The type of entity this hook must watch for.</typeparam>
    public abstract class PreActionHook<TEntity> : IPreActionHook
    {
        public abstract string Name { get; }
        public virtual int Order => default;

        /// <summary>
        /// Entity States that this hook must be registered to listen for.
        /// </summary>
        public abstract EntityState HookState { get; }

        /// <summary>
        /// Implements the interface.  This causes the hook to only run for objects that are assignable to TEntity.
        /// </summary>
        public void Hook(object entity, HookEntityMetadata metadata, IUnitOfWork uow)
        {
            if (entity is TEntity typedEntity)
                Hook(typedEntity, metadata, uow);
        }

        /// <summary>
        /// The logic to perform per entity before the registered action gets performed.
        /// This gets run once per entity that has been changed.
        /// </summary>
        protected abstract void Hook(TEntity entity, HookEntityMetadata metadata, IUnitOfWork uow);
    }
}