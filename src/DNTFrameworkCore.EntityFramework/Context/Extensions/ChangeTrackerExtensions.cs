using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context.Extensions
{
    public static class ChangeTrackerExtensions
    {
        /// <summary>
        /// Find the base types of the given type, recursively.
        /// </summary>
        public static IEnumerable<Type> FindBaseTypes(this Type type)
        {
            if (type.GetTypeInfo().BaseType == null)
            {
                return type.GetInterfaces();
            }

            return Enumerable.Repeat(type.GetTypeInfo().BaseType, 1)
                .Concat(type.GetInterfaces())
                .Concat(type.GetInterfaces().SelectMany(FindBaseTypes))
                .Concat(type.GetTypeInfo().BaseType.FindBaseTypes());
        }

        /// <summary>
        /// Using the ChangeTracker to find names of the changed entities.
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
        /// Using the ChangeTracker to find types of the changed entities.
        /// </summary>
        public static IEnumerable<Type> FindChangedEntityTypes(this DbContext dbContext)
        {
            return dbContext.ChangeTracker.Entries().Where(
                    dbEntityEntry => dbEntityEntry.State == EntityState.Added ||
                                     dbEntityEntry.State == EntityState.Modified ||
                                     dbEntityEntry.State == EntityState.Deleted)
                .Select(dbEntityEntry => dbEntityEntry.Entity.GetType());
        }
    }
}