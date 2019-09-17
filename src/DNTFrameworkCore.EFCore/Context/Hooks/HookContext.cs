namespace DNTFrameworkCore.EFCore.Context.Hooks
{
    public class HookContext<TEntity>
    {
        /// <summary>
        /// The entity that is processed by Entity Framework
        /// </summary>
        public TEntity Entity { get; }
        /// <summary>
        /// Metadata about the entity in the context of this hook - such as state
        /// </summary>
        public HookEntityMetadata Metadata { get; }
        /// <summary>
        /// Current context instance
        /// </summary>
        public IUnitOfWork Context { get; }

        public HookContext(TEntity entity, HookEntityMetadata metadata, IUnitOfWork context)
        {
            Entity = entity;
            Metadata = metadata;
            Context = context;
        }
    }
}