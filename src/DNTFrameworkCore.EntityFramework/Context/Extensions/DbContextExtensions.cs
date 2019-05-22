using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.ReflectionToolkit;
using DNTPersianUtils.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DNTFrameworkCore.EntityFramework.Context.Extensions
{
    public static class DbContextExtensions
    {
        /// <summary>
        /// More info: http://www.dotnettips.info/post/465
        /// And http://www.dotnettips.info/post/2507
        /// </summary>
        public static void ApplyCorrectYeKe(this DbContext dbContext)
        {
            if (dbContext == null)
            {
                return;
            }

            var changedEntities = dbContext.ChangeTracker
                .Entries()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified);

            foreach (var item in changedEntities)
            {
                var entity = item.Entity;
                if (item.Entity == null)
                {
                    continue;
                }

                var propertyInfos = entity.GetType().GetProperties(
                    BindingFlags.Public | BindingFlags.Instance
                ).Where(p => p.CanRead && p.CanWrite && p.PropertyType == typeof(string));

                var propertyReflector = new PropertyReflector();

                foreach (var propertyInfo in propertyInfos)
                {
                    var propName = propertyInfo.Name;
                    var value = propertyReflector.GetValue(entity, propName);
                    if (value != null)
                    {
                        var strValue = value.ToString();
                        var newVal = strValue.ApplyCorrectYeKe();
                        if (newVal == strValue)
                        {
                            continue;
                        }

                        propertyReflector.SetValue(entity, propName, newVal);
                    }
                }
            }
        }

        public static void ValidateEntities(this DbContext context)
        {
            var errors = context.GetValidationErrors();
            if (string.IsNullOrWhiteSpace(errors)) return;

            var message = $"There are some validation errors while saving changes in EntityFramework:\n {errors}";

            throw new InvalidOperationException(message);
        }

        private static string GetValidationErrors(this DbContext context)
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

        public static IReadOnlyList<HookedEntityEntry> FindChangedEntries(this DbContext context)
        {
            return context.ChangeTracker.Entries<IEntity>()
                .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified ||
                            x.State == EntityState.Deleted)
                .Select(x => new HookedEntityEntry
                {
                    Entity = x.Entity,
                    PreSaveState = x.State
                })
                .ToList();
        }

        /// <summary>
        /// Set entity state to Detached for entities in more than one object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement IHasTrackingState</param>
        public static void DetachEntities(this DbContext context, IEnumerable<IHasTrackingState> items)
        {
            foreach (var item in items)
                context.DetachEntities(item);
        }

        /// <summary>
        /// Set entity state to Detached for entities in an object graph.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        public static void DetachEntities(this DbContext context, IHasTrackingState item)
        {
            context.TraverseGraph(item, n => n.Entry.State = EntityState.Detached);
        }

        /// <summary>
        /// Traverse an object graph to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        public static void LoadRelatedEntities(this DbContext context, IHasTrackingState item)
        {
            context.TraverseGraph(item, n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                {
                    if (!reference.IsLoaded)
                        reference.Load();
                }
            });
        }

        /// <summary>
        /// Traverse more than one object graph to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement IHasTrackingState</param>
        public static void LoadRelatedEntities(this DbContext context, IEnumerable<IHasTrackingState> items)
        {
            foreach (var item in items)
                context.LoadRelatedEntities(item);
        }

        /// <summary>
        /// Traverse an object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        public static async Task LoadRelatedEntitiesAsync(this DbContext context, IHasTrackingState item)
        {
            await context.TraverseGraphAsync(item, async n =>
            {
                if (n.Entry.State == EntityState.Detached)
                    n.Entry.State = EntityState.Unchanged;
                foreach (var reference in n.Entry.References)
                {
                    if (!reference.IsLoaded)
                        await reference.LoadAsync();
                }
            });
        }

        /// <summary>
        /// Traverse more than one object graph asynchronously to populate null reference properties.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="items">Objects that implement IHasTrackingState</param>
        public static async Task LoadRelatedEntitiesAsync(this DbContext context, IEnumerable<IHasTrackingState> items)
        {
            foreach (var item in items)
                await context.LoadRelatedEntitiesAsync(item);
        }

        /// <summary>
        /// Traverse an object graph executing a callback on each node.
        /// </summary>
        /// <param name="context">Used to query and save changes to a database</param>
        /// <param name="item">Object that implements IHasTrackingState</param>
        /// <param name="callback">Callback executed on each node in the object graph</param>
        public static void TraverseGraph(this DbContext context, object item,
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
        /// Traverse an object graph asynchronously executing a callback on each node.
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