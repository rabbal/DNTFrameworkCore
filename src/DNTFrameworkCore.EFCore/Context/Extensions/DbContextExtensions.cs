using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
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
    public static class DbContextExtensions
    {
        public static void EntityVersion(this IDbContext dbContext, IHasRowVersion versionedEntity, byte[] version)
        {
            dbContext.PropertyValue(versionedEntity, EFCoreShadow.Version, version);
        }

        public static byte[] EntityVersion(this IDbContext dbContext, IHasRowVersion versionedEntity)
        {
            return (byte[]) dbContext.PropertyValue(versionedEntity, EFCoreShadow.Version);
        }

        public static string EntityHash(this IDbContext dbContext, IHasRowIntegrity entity)
        {
            return dbContext.PropertyValue<string>(entity, EFCoreShadow.Hash);
        }

        public static bool IsTampered<TEntity>(this IDbContext dbContext, TEntity entity)
            where TEntity : class, IHasRowIntegrity
        {
            return dbContext.EntityHash(entity) != dbContext.PropertyValue<string>(entity, EFCoreShadow.Hash);
        }

        public static async Task<bool> IsTamperedAsync<TEntity, TKey>(this IDbContext dbContext, TKey id)
            where TEntity : Entity<TKey>, IHasRowIntegrity
            where TKey : IEquatable<TKey>

        {
            var entity = await dbContext.Set<TEntity>().FindAsync(id);
            return dbContext.EntityHash(entity) != dbContext.PropertyValue<string>(entity, EFCoreShadow.Hash);
        }

        public static async Task<bool> HasTamperedEntryAsync<TEntity>(this IDbContext dbContext,
            Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, IHasRowIntegrity
        {
            var tamperedEntryList = await dbContext.TamperedEntryListAsync(predicate);
            return tamperedEntryList.Any();
        }

        public static async Task<IReadOnlyList<TEntity>> TamperedEntryListAsync<TEntity>(this IDbContext dbContext,
            Expression<Func<TEntity, bool>> predicate = null)
            where TEntity : class, IHasRowIntegrity
        {
            var entityList = await dbContext.Set<TEntity>()
                //.AsNoTracking() todo: shadow-property (Hash)
                .WhereIf(predicate != null, predicate)
                .ToListAsync();

            return entityList
                .Where(entity =>
                    dbContext.EntityHash(entity) != dbContext.PropertyValue<string>(entity, EFCoreShadow.Hash))
                .ToList();
        }

        public static void IgnoreRowIntegrityHook(this IDbContext dbContext)
        {
            dbContext.IgnoreHook(HookNames.RowIntegrity);
        }

        public static void IgnoreNumberingHook(this IDbContext dbContext)
        {
            dbContext.IgnoreHook(HookNames.Numbering);
        }

        public static void IgnoreTrackingHook(this IDbContext dbContext)
        {
            dbContext.IgnoreHook(HookNames.CreationTracking);
            dbContext.IgnoreHook(HookNames.ModificationTracking);
        }

        public static T PropertyValue<T>(this IDbContext dbContext, object entity, string propertyName)
            where T : IConvertible
        {
            var value = dbContext.Entry(entity).Property(propertyName).CurrentValue;
            return value != null ? value.To<T>() : default;
        }

        public static object PropertyValue(this IDbContext dbContext, object entity, string propertyName)
        {
            return dbContext.Entry(entity).Property(propertyName).CurrentValue;
        }

        public static void PropertyValue(this IDbContext dbContext, object entity, string propertyName,
            object propertyValue)
        {
            dbContext.Entry(entity).Property(propertyName).CurrentValue = propertyValue;
        }

        public static TResult RunInTransaction<TResult>(this IDbContext dbContext, Func<TResult> action,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TResult result;
            try
            {
                dbContext.BeginTransaction(isolationLevel);
                result = action.Invoke();
                dbContext.CommitTransaction();
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }

            return result;
        }

        public static async Task<TResult> RunInTransactionAsync<TResult>(this IDbContext dbContext,
            Func<Task<TResult>> action,
            CancellationToken cancellationToken = default,
            IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
        {
            TResult result;
            try
            {
                await dbContext.BeginTransactionAsync(isolationLevel, cancellationToken);
                result = await action.Invoke();
                await dbContext.CommitTransactionAsync(cancellationToken);
            }
            catch (Exception)
            {
                dbContext.RollbackTransaction();
                throw;
            }

            return result;
        }

        /// <summary>
        /// Begins tracking a collection of entity and any entities that are reachable by traversing their navigation properties.
        /// This method is designed for use in disconnected scenarios.
        /// </summary>
        internal static void UpdateGraph(this IDbContext dbContext, IEnumerable<object> rootEntities)
        {
            foreach (var rootEntity in rootEntities)
                dbContext.UpdateGraph(rootEntity);
        }

        /// <summary>
        /// Begins tracking an entity and any entities that are reachable by traversing it's navigation properties.
        /// This method is designed for use in disconnected scenarios.
        /// </summary>
        internal static void UpdateGraph(this IDbContext dbContext, object rootEntity)
        {
            dbContext.Entry(rootEntity).State = EntityState.Detached;

            dbContext.TrackGraph(rootEntity, node =>
            {
                if (node.Entry.Entity is ITrackable trackable)
                {
                    SetEntityState(node, trackable);
                }
                else
                {
                    dbContext.Entry(rootEntity).State = EntityState.Modified;
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
        public static void LoadRelatedEntities(this IDbContext dbContext, object entity)
        {
            dbContext.TraverseGraph(entity, n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                    if (!reference.IsLoaded && reference.CurrentValue == null)
                        reference.Load();
            });
        }

        /// <summary>
        ///     Traverse more than one object graph to populate null reference properties.
        /// </summary>
        public static void LoadRelatedEntities(this IDbContext dbContext, IEnumerable<object> entities)
        {
            foreach (var entity in entities)
                dbContext.LoadRelatedEntities(entity);
        }

        /// <summary>
        ///     Traverse an object graph asynchronously to populate null reference properties.
        /// </summary>
        public static async Task LoadRelatedEntitiesAsync(this IDbContext dbContext, object entity)
        {
            await dbContext.TraverseGraphAsync(entity, async n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                    if (!reference.IsLoaded && reference.CurrentValue == null)
                        await reference.LoadAsync();
            });
        }

        /// <summary>
        ///     Traverse more than one object graph asynchronously to populate null reference properties.
        /// </summary>
        public static async Task LoadRelatedEntitiesAsync(this IDbContext dbContext, IEnumerable<object> entities)
        {
            foreach (var entity in entities)
                await dbContext.LoadRelatedEntitiesAsync(entity);
        }

        /// <summary>
        ///     Traverse an object graph to set TrackingState to Unchanged.
        /// </summary>
        public static void MarkUnchanged(this IDbContext dbContext, object rootEntity)
        {
            dbContext.TraverseGraph(rootEntity, n =>
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
        public static void MarkUnchanged(this IDbContext dbContext, IEnumerable<object> items)
        {
            foreach (var item in items)
                dbContext.MarkUnchanged(item);
        }


        /// <summary>
        /// Traverse an object graph executing a callback on each node.
        /// </summary>
        private static void TraverseGraph(this IDbContext context, object entity,
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
        private static async Task TraverseGraphAsync(this IDbContext context, object item,
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

        /// <summary>
        /// Using the ChangeTracker to find names of the changed entities.
        /// </summary>
        public static IEnumerable<string> FindChangedEntityNames(this DbContext dbContext)
        {
            return dbContext.FindChangedEntries().FindEntityNames();
        }

        /// <summary>
        /// Using the ChangeTracker to find names of the changed entities.
        /// </summary>
        public static IEnumerable<string> FindEntityNames(this IEnumerable<EntityEntry> entryList)
        {
            var typesList = new List<Type>();
            foreach (var type in entryList.Select(entry => entry.Entity.GetType()))
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
        /// Using the ChangeTracker to find types of the changed entities.
        /// </summary>
        public static IEnumerable<Type> FindChangedEntityTypes(this DbContext dbContext)
        {
            return dbContext.FindChangedEntries()
                .Select(dbEntityEntry => dbEntityEntry.Entity.GetType());
        }

        /// <summary>
        /// Find the base types of the given type, recursively.
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
    }
}