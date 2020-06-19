using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DNTFrameworkCore.EFCore.Context.Extensions
{
    public static class UnitOfWorkExtensions
    {
        public static void EntityVersion(this IUnitOfWork uow, IRowVersion versionedEntity, byte[] version)
        {
            uow.PropertyValue(versionedEntity, EFCore.Version, version);
        }

        public static byte[] EntityVersion(this IUnitOfWork uow, IRowVersion versionedEntity)
        {
            return (byte[]) uow.PropertyValue(versionedEntity, EFCore.Version);
        }

        public static string EntityHash(this IUnitOfWork uow, IRowIntegrity entity)
        {
            return uow.PropertyValue<string>(entity, EFCore.Hash);
        }

        public static bool IsTampered<TEntity>(this IUnitOfWork uow, TEntity entity)
            where TEntity : class, IRowIntegrity
        {
            return uow.EntityHash(entity) != uow.PropertyValue<string>(entity, EFCore.Hash);
        }

        public static async Task<bool> IsTamperedAsync<TEntity, TKey>(this IUnitOfWork uow, TKey id)
            where TEntity : Entity<TKey>, IRowIntegrity
            where TKey : IEquatable<TKey>

        {
            var entity = await uow.Set<TEntity>().FindAsync(id);
            return uow.EntityHash(entity) != uow.PropertyValue<string>(entity, EFCore.Hash);
        }

        public static async Task<bool> HasTamperedEntryAsync<TEntity>(this IUnitOfWork uow,
            Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, IRowIntegrity
        {
            var tamperedEntryList = await uow.TamperedEntryListAsync(predicate);
            return tamperedEntryList.Any();
        }

        public static async Task<IReadOnlyList<TEntity>> TamperedEntryListAsync<TEntity>(this IUnitOfWork uow,
            Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, IRowIntegrity
        {
            var entityList = await uow.Set<TEntity>()
                //.AsNoTracking() todo: shadow-property (Hash)
                .WhereIf(predicate != null, predicate)
                .ToListAsync();

            return entityList
                .Where(entity => uow.EntityHash(entity) != uow.PropertyValue<string>(entity, EFCore.Hash))
                .ToList();
        }

        public static void IgnoreRowIntegrityHook(this IUnitOfWork uow)
        {
            uow.IgnoreHook(HookNames.RowIntegrity);
        }

        public static void IgnoreNumberingHook(this IUnitOfWork uow)
        {
            uow.IgnoreHook(HookNames.Numbering);
        }

        public static void IgnoreTrackingHook(this IUnitOfWork uow)
        {
            uow.IgnoreHook(HookNames.CreationTracking);
            uow.IgnoreHook(HookNames.ModificationTracking);
        }

        public static T PropertyValue<T>(this IUnitOfWork uow, object entity, string propertyName)
            where T : IConvertible
        {
            var value = uow.Entry(entity).Property(propertyName).CurrentValue;
            return value != null ? value.To<T>() : default;
        }

        public static object PropertyValue(this IUnitOfWork uow, object entity, string propertyName)
        {
            return uow.Entry(entity).Property(propertyName).CurrentValue;
        }

        public static void PropertyValue(this IUnitOfWork uow, object entity, string propertyName,
            object propertyValue)
        {
            uow.Entry(entity).Property(propertyName).CurrentValue = propertyValue;
        }

        public static TResult RunInTransaction<TResult>(this IUnitOfWork uow, Func<TResult> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TResult result;
            try
            {
                uow.BeginTransaction(isolationLevel);
                result = action.Invoke();
                uow.CommitTransaction();
            }
            catch (Exception)
            {
                uow.RollbackTransaction();
                throw;
            }

            return result;
        }

        public static async Task<TResult> RunInTransactionAsync<TResult>(this IUnitOfWork uow,
            Func<Task<TResult>> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TResult result;
            try
            {
                await uow.BeginTransactionAsync(isolationLevel);
                result = await action.Invoke();
                uow.CommitTransaction();
            }
            catch (Exception)
            {
                uow.RollbackTransaction();
                throw;
            }

            return result;
        }

        /// <summary>
        /// Begins tracking a collection of entity and any entities that are reachable by traversing their navigation properties.
        /// This method is designed for use in disconnected scenarios.
        /// </summary>
        internal static void UpdateGraph(this IUnitOfWork uow, IEnumerable<object> rootEntities)
        {
            foreach (var rootEntity in rootEntities)
                uow.UpdateGraph(rootEntity);
        }

        /// <summary>
        /// Begins tracking an entity and any entities that are reachable by traversing it's navigation properties.
        /// This method is designed for use in disconnected scenarios.
        /// </summary>
        internal static void UpdateGraph(this IUnitOfWork uow, object rootEntity)
        {
            uow.Entry(rootEntity).State = EntityState.Detached;

            uow.TrackGraph(rootEntity, node =>
            {
                if (node.Entry.Entity is ITrackable trackable)
                {
                    SetEntityState(node, trackable);
                }
                else
                {
                    uow.Entry(rootEntity).State = EntityState.Modified;
                }
            });
        }

        private static void SetEntityState(this EntityEntryGraphNode node, ITrackable trackable)
        {
            node.Entry.State = EntityState.Detached;

            if (node.SourceEntry != null)
            {
                var relationship = node.InboundNavigation?.ToRelationshipType();
                switch (relationship)
                {
                    case RelationshipType.OneToOne:
                        // If parent is added set to added
                        if (node.SourceEntry.State == EntityState.Added)
                            SetEntityState(node.Entry, TrackingState.Added.ToEntityState(), trackable);
                        else if (node.SourceEntry.State == EntityState.Deleted)
                            SetEntityState(node.Entry, TrackingState.Deleted.ToEntityState(), trackable);
                        else
                            SetEntityState(node.Entry, trackable.TrackingState.ToEntityState(), trackable);

                        return;
                    case RelationshipType.ManyToOne:
                        // If parent is added set to added
                        if (node.SourceEntry.State == EntityState.Added)
                        {
                            SetEntityState(node.Entry, TrackingState.Added.ToEntityState(), trackable);
                            return;
                        }

                        // If parent is deleted set to deleted
                        var parent = node.SourceEntry.Entity as ITrackable;
                        if (node.SourceEntry.State == EntityState.Deleted
                            || parent?.TrackingState == TrackingState.Deleted)
                        {
                            try
                            {
                                // Will throw if there are added children
                                SetEntityState(node.Entry, TrackingState.Deleted.ToEntityState(), trackable);
                            }
                            catch (InvalidOperationException e)
                            {
                                throw new InvalidOperationException(
                                    @"An entity may not be marked as Deleted if it has related entities which are marked as Added. 
                                        Remove added related entities before deleting a parent entity.", e);
                            }

                            return;
                        }

                        break;
                    case RelationshipType.OneToMany:
                        // If trackable is set deleted set entity state to unchanged,
                        // since it may be related to other entities.
                        if (trackable.TrackingState == TrackingState.Deleted)
                        {
                            SetEntityState(node.Entry, TrackingState.Unchanged.ToEntityState(), trackable);
                            return;
                        }

                        break;
                }
            }

            SetEntityState(node.Entry, trackable.TrackingState.ToEntityState(), trackable);
        }

        /// <summary>
        ///     Traverse an object graph to populate null reference properties.
        /// </summary>
        public static void LoadRelatedEntities(this IUnitOfWork uow, object entity)
        {
            uow.TraverseGraph(entity, n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                    if (!reference.IsLoaded)
                        reference.Load();
            });
        }

        /// <summary>
        ///     Traverse more than one object graph to populate null reference properties.
        /// </summary>
        public static void LoadRelatedEntities(this IUnitOfWork uow, IEnumerable<object> entities)
        {
            foreach (var entity in entities)
                uow.LoadRelatedEntities(entity);
        }

        /// <summary>
        ///     Traverse an object graph asynchronously to populate null reference properties.
        /// </summary>
        public static async Task LoadRelatedEntitiesAsync(this IUnitOfWork uow, object entity)
        {
            await uow.TraverseGraphAsync(entity, async n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                    if (!reference.IsLoaded)
                        await reference.LoadAsync();
            });
        }

        /// <summary>
        ///     Traverse more than one object graph asynchronously to populate null reference properties.
        /// </summary>
        public static async Task LoadRelatedEntitiesAsync(this IUnitOfWork uow, IEnumerable<object> entities)
        {
            foreach (var entity in entities)
                await uow.LoadRelatedEntitiesAsync(entity);
        }

        /// <summary>
        ///     Traverse an object graph to set TrackingState to Unchanged.
        /// </summary>
        public static void MarkUnchanged(this IUnitOfWork uow, object rootEntity)
        {
            uow.TraverseGraph(rootEntity, n =>
            {
                if (n.Entry.Entity is ITrackable trackable)
                {
                    trackable.Unchange();
                }
            });
        }

        /// <summary>
        ///     Traverse more than one object graph to set TrackingState to Unchanged.
        /// </summary>
        public static void MarkUnchanged(this IUnitOfWork uow, IEnumerable<object> items)
        {
            foreach (var item in items)
                uow.MarkUnchanged(item);
        }


        /// <summary>
        /// Traverse an object graph executing a callback on each node.
        /// </summary>
        private static void TraverseGraph(this IUnitOfWork context, object entity,
            Action<EntityEntryGraphNode> callback)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            var stateManager = context.Entry(entity).GetInfrastructure().StateManager;
            var node = new EntityEntryGraphNode<object>(stateManager.GetOrCreateEntry(entity), null, null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
#pragma warning restore EF1001 // Internal EF Core API usage.
            var visited = new HashSet<int>();

            graphIterator.TraverseGraph(node, n =>
            {
                // Check visited
                if (visited.Contains(n.Entry.Entity.GetHashCode()))
                    return false;

                // Execute callback
                callback(n);

                // Add visited
                visited.Add(n.Entry.Entity.GetHashCode());

                // Continue traversal
                return true;
            });
        }

        /// <summary>
        /// Traverse an object graph asynchronously executing a callback on each node.
        /// </summary>
        private static async Task TraverseGraphAsync(this IUnitOfWork context, object item,
            Func<EntityEntryGraphNode, Task> callback)
        {
#pragma warning disable EF1001 // Internal EF Core API usage.
            var stateManager = context.Entry(item).GetInfrastructure().StateManager;
            var node = new EntityEntryGraphNode<object>(stateManager.GetOrCreateEntry(item), null, null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
#pragma warning restore EF1001 // Internal EF Core API usage.
            var visited = new HashSet<int>();

            await graphIterator.TraverseGraphAsync(node, async (n, ct) =>
            {
                // Check visited
                if (visited.Contains(n.Entry.Entity.GetHashCode()))
                    return false;

                // Execute callback
                await callback(n);

                // Add visited
                visited.Add(n.Entry.Entity.GetHashCode());

                // Continue traversal
                return true;
            });
        }

        private static void SetEntityState(EntityEntry entry, EntityState state, ITrackable trackable)
        {
            entry.State = state;

            if (entry.State != EntityState.Modified || trackable.ModifiedProperties == null) return;

            foreach (var property in entry.Properties)
                property.IsModified = trackable.ModifiedProperties.Any(p =>
                    string.Compare(p, property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}