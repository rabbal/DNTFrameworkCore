using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DNTFrameworkCore.EFCore.Context.Extensions
{
    public static class UnitOfWorkExtensions
    {
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

        public static void ApplyChanges(this IUnitOfWork uow, ITrackable item)
        {
            // Detach root entity
            uow.Entry(item).State = EntityState.Detached;

            // Recursively set entity state for DbContext entry
            uow.TrackGraph(item, node =>
            {
                // Exit if not ITrackable
                if (!(node.Entry.Entity is ITrackable trackable)) return;

                // Detach node entity
                node.Entry.State = EntityState.Detached;

                // Get related parent entity
                if (node.SourceEntry != null)
                {
                    var relationship = node.InboundNavigation?.GetRelationshipType();
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

                // Set entity state to tracking state
                SetEntityState(node.Entry, trackable.TrackingState.ToEntityState(), trackable);
            });
        }

        /// <summary>
        ///     Update entity state on DbContext for more than one object graph.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void ApplyChanges(this IUnitOfWork uow, IEnumerable<ITrackable> items)
        {
            // Apply changes to collection of items
            foreach (var item in items)
                uow.ApplyChanges(item);
        }

        /// <summary>
        ///     Set entity state to Detached for entities in more than one object graph.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void DetachEntities(this IUnitOfWork uow, IEnumerable<ITrackable> items)
        {
            // Detach each item in the object graph
            foreach (var item in items)
                uow.DetachEntities(item);
        }

        /// <summary>
        ///     Set entity state to Detached for entities in an object graph.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void DetachEntities(this IUnitOfWork uow, ITrackable item)
        {
            // Detach each item in the object graph
            uow.TraverseGraph(item, n => n.Entry.State = EntityState.Detached);
        }

        /// <summary>
        ///     Traverse an object graph to populate null reference properties.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void LoadRelatedEntities(this IUnitOfWork uow, ITrackable item)
        {
            // Traverse graph to load references          
            uow.TraverseGraph(item, n =>
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
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void LoadRelatedEntities(this IUnitOfWork uow, IEnumerable<ITrackable> items)
        {
            // Traverse graph to load references          
            foreach (var item in items)
                uow.LoadRelatedEntities(item);
        }

        /// <summary>
        ///     Traverse an object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static async Task LoadRelatedEntitiesAsync(this IUnitOfWork uow, ITrackable item)
        {
            // Detach each item in the object graph         
            await uow.TraverseGraphAsync(item, async n =>
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
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static async Task LoadRelatedEntitiesAsync(this IUnitOfWork uow, IEnumerable<ITrackable> items)
        {
            // Traverse graph to load references
            foreach (var item in items)
                await uow.LoadRelatedEntitiesAsync(item);
        }

        /// <summary>
        ///     Traverse an object graph to set TrackingState to Unchanged.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements ITrackable</param>
        public static void AcceptChanges(this IUnitOfWork uow, ITrackable item)
        {
            // Traverse graph to set TrackingState to Unchanged
            uow.TraverseGraph(item, n =>
            {
                if (n.Entry.Entity is ITrackable trackable)
                {
                    if (trackable.TrackingState != TrackingState.Unchanged)
                        trackable.TrackingState = TrackingState.Unchanged;
                    if (trackable.ModifiedProperties?.Count > 0)
                        trackable.ModifiedProperties.Clear();
                }
            });
        }

        /// <summary>
        ///     Traverse more than one object graph to set TrackingState to Unchanged.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement ITrackable</param>
        public static void AcceptChanges(this IUnitOfWork uow, IEnumerable<ITrackable> items)
        {
            // Traverse graph to set TrackingState to Unchanged
            foreach (var item in items)
                uow.AcceptChanges(item);
        }


        /// <summary>
        ///     Traverse an object graph executing a callback on each node.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        /// <param name="callback">Callback executed on each node in the object graph</param>
        public static void TraverseGraph(this IUnitOfWork uow, object item,
            Action<EntityEntryGraphNode> callback)
        {
            var stateManager = uow.Entry(item).GetInfrastructure().StateManager;
            var node = new EntityEntryGraphNode(stateManager.GetOrCreateEntry(item), null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
            var visited = new HashSet<int>();

            graphIterator.TraverseGraph<object>(node, null, (n, s) =>
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
        ///     Traverse an object graph asynchronously executing a callback on each node.
        /// </summary>
        /// <param name="uow">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        /// <param name="callback">Async callback executed on each node in the object graph</param>
        private static async Task TraverseGraphAsync(this IUnitOfWork uow, object item,
            Func<EntityEntryGraphNode, Task> callback)
        {
            var stateManager = uow.Entry(item).GetInfrastructure().StateManager;
            var node = new EntityEntryGraphNode(stateManager.GetOrCreateEntry(item), null, null);
            IEntityEntryGraphIterator graphIterator = new EntityEntryGraphIterator();
            var visited = new HashSet<int>();

            await graphIterator.TraverseGraphAsync<object>(node, null, async (n, s, ct) =>
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
            // Set entity state to tracking state
            entry.State = state;

            if (entry.State != EntityState.Modified || trackable.ModifiedProperties == null) return;

            // Set modified properties
            foreach (var property in entry.Properties)
                property.IsModified = trackable.ModifiedProperties.Any(p =>
                    string.Compare(p, property.Metadata.Name, StringComparison.InvariantCultureIgnoreCase) == 0);
        }
    }
}