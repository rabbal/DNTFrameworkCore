using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DNTFrameworkCore.EFCore.Context.Extensions
{
    public static class DbContextExtensions
    {
        public static void ApplyChanges(this IDbContext context, IEnumerable<ITrackable> roots)
        {
            foreach (var root in roots)
                context.ApplyChanges(root);
        }

        public static void AcceptChanges(this IDbContext context, IEnumerable<ITrackable> roots)
        {
            foreach (var root in roots)
                context.AcceptChanges(root);
        }

        public static TResult RunInTransaction<TResult>(this IDbContext context, Func<TResult> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TResult result;
            try
            {
                context.BeginTransaction(isolationLevel);
                result = action.Invoke();
                context.CommitTransaction();
            }
            catch (Exception)
            {
                context.RollbackTransaction();
                throw;
            }
            
            return result;
        }

        public static async Task<TResult> RunInTransactionAsync<TResult>(this IDbContext context,
            Func<Task<TResult>> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TResult result;
            try
            {
                await context.BeginTransactionAsync(isolationLevel);
                result = await action.Invoke();
                context.CommitTransaction();
            }
            catch (Exception)
            {
                context.RollbackTransaction();
                throw;
            }
            
            return result;
        }

        public static void ApplyChanges(this IDbContext context, ITrackable root)
        {
            context.TrackGraph(root, node =>
            {
                if (!(node.Entry.Entity is ITrackable hasTrackingState)) return;

                node.Entry.State = EntityState.Detached;

                // Get related parent entity
                if (node.SourceEntry != null)
                {
                    var relationship = node.InboundNavigation?.GetRelationshipType();
                    switch (relationship)
                    {
                        case RelationshipType.OneToOne:
                            if (node.SourceEntry.State == EntityState.Added)
                                node.Entry.State = TrackingState.Added.ToEntityState();
                            else if (node.SourceEntry.State == EntityState.Deleted)
                                node.Entry.State = TrackingState.Deleted.ToEntityState();
                            else
                                node.Entry.State = hasTrackingState.TrackingState.ToEntityState();

                            return;
                        case RelationshipType.ManyToOne:
                            if (node.SourceEntry.State == EntityState.Added)
                            {
                                node.Entry.State = TrackingState.Added.ToEntityState();
                                return;
                            }

                            var parent = node.SourceEntry.Entity as ITrackable;
                            if (node.SourceEntry.State == EntityState.Deleted
                                || parent?.TrackingState == TrackingState.Deleted)
                            {
                                try
                                {
                                    node.Entry.State = TrackingState.Deleted.ToEntityState();
                                }
                                catch (InvalidOperationException e)
                                {
                                    throw new InvalidOperationException(
                                        @"An entity may not be marked as Deleted if it has related entities which are marked as Added.
                                            Remove added related entities before deleting a parent entity.",
                                        e);
                                }

                                return;
                            }

                            break;
                        case RelationshipType.OneToMany:
                            // If ITrackedEntity is set deleted set entity state to unchanged,
                            // since it may be related to other entities.
                            if (hasTrackingState.TrackingState == TrackingState.Deleted)
                            {
                                node.Entry.State = TrackingState.Unchanged.ToEntityState();
                                return;
                            }

                            break;
                    }
                }

                node.Entry.State = hasTrackingState.TrackingState.ToEntityState();
            });
        }

        public static void AcceptChanges(this IDbContext context, ITrackable root)
        {
            context.TraverseGraph(root, n =>
            {
                if (!(n.Entry.Entity is ITrackable hasTrackingState)) return;

                if (hasTrackingState.TrackingState != TrackingState.Unchanged)
                    hasTrackingState.TrackingState = TrackingState.Unchanged;
            });
        }

        /// <summary>
        ///     Using the ChangeTracker to find names of the changed entities.
        /// </summary>
        public static IEnumerable<string> FindChangedEntityNames(this DbContext dbContext)
        {
            var typesList = new List<Type>();
            foreach (var type in dbContext.FindChangedEntityTypes())
            {
                typesList.Add(type);
                typesList.AddRange(type.FindBaseTypes().Where(t => t != typeof(object)).ToList());
            }

            var changedEntityNames = typesList
                .Select(type => type.FullName)
                .Distinct()
                .ToArray();

            return changedEntityNames;
        }

        /// <summary>
        ///     Using the ChangeTracker to find types of the changed entities.
        /// </summary>
        public static IEnumerable<Type> FindChangedEntityTypes(this DbContext dbContext)
        {
            return dbContext.FindChangedEntries()
                .Select(dbEntityEntry => dbEntityEntry.Entity.GetType());
        }

        /// <summary>
        ///     Find the base types of the given type, recursively.
        /// </summary>
        private static IEnumerable<Type> FindBaseTypes(this Type type)
        {
            if (type.GetTypeInfo().BaseType == null) return type.GetInterfaces();

            return Enumerable.Repeat(type.GetTypeInfo().BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.GetInterfaces().SelectMany(FindBaseTypes))
                .Concat(type.GetTypeInfo().BaseType.FindBaseTypes());
        }

        public static void ThrowIfInvalidEntityExist(this DbContext context)
        {
            var errors = context.FindValidationErrors();
            if (string.IsNullOrWhiteSpace(errors)) return;

            var message = $"There are some validation errors while saving changes in EntityFramework:\n {errors}";

            throw new InvalidOperationException(message);
        }

        private static string FindValidationErrors(this DbContext context)
        {
            var errors = new StringBuilder();
            var entities = context.ChangeTracker.Entries()
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified)
                .Select(e => e.Entity);
            foreach (var entity in entities)
            {
                var validationContext = new ValidationContext(entity);
                var validationResults = new List<ValidationResult>();

                if (Validator.TryValidateObject(entity, validationContext, validationResults, true)) continue;

                foreach (var validationResult in validationResults)
                {
                    var names = validationResult.MemberNames.Aggregate((s1, s2) => $"{s1}, {s2}");
                    errors.AppendFormat("{0}: {1}", names, validationResult.ErrorMessage);
                }
            }

            return errors.ToString();
        }

        public static IReadOnlyList<EntityEntry> FindChangedEntries(this DbContext context)
        {
            return context.ChangeTracker.Entries()
                .Where(x =>
                    x.State == EntityState.Added ||
                    x.State == EntityState.Modified ||
                    x.State == EntityState.Deleted)
                .ToList();
        }

        /// <summary>
        ///     Set entity state to Detached for entities in more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement IHasTrackingState</param>
        public static void DetachEntities(this IDbContext context, IEnumerable<ITrackable> items)
        {
            foreach (var item in items)
                context.DetachEntities(item);
        }

        /// <summary>
        ///     Set entity state to Detached for entities in an object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        public static void DetachEntities(this IDbContext context, ITrackable item)
        {
            context.TraverseGraph(item, n => n.Entry.State = EntityState.Detached);
        }

        /// <summary>
        ///     Traverse an object graph to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        public static void LoadRelatedEntities(this IDbContext context, ITrackable item)
        {
            context.TraverseGraph(item, n =>
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
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement IHasTrackingState</param>
        public static void LoadRelatedEntities(this IDbContext context, IEnumerable<ITrackable> items)
        {
            foreach (var item in items)
                context.LoadRelatedEntities(item);
        }

        /// <summary>
        ///     Traverse an object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        public static async Task LoadRelatedEntitiesAsync(this DbContext context, ITrackable item)
        {
            await context.TraverseGraphAsync(item, async n =>
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
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement IHasTrackingState</param>
        public static async Task LoadRelatedEntitiesAsync(this DbContext context, IEnumerable<ITrackable> items)
        {
            foreach (var item in items)
                await context.LoadRelatedEntitiesAsync(item);
        }

        /// <summary>
        ///     Traverse an object graph executing a callback on each node.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        /// <param name="callback">Callback executed on each node in the object graph</param>
        public static void TraverseGraph(this IDbContext context, object item,
            Action<EntityEntryGraphNode> callback)
        {
            var stateManager = context.Entry(item).GetInfrastructure().StateManager;
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
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        /// <param name="callback">Async callback executed on each node in the object graph</param>
        private static async Task TraverseGraphAsync(this DbContext context, object item,
            Func<EntityEntryGraphNode, Task> callback)
        {
            var stateManager = context.Entry(item).GetInfrastructure().StateManager;
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
    }
}