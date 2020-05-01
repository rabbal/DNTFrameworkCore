using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Exceptions;

namespace DNTFrameworkCore.Mapping
{
    public static class MappingExtensions
    {
        public static void Map<TSource, TDestination>(IReadOnlyList<TSource> source,
            IEnumerable<TDestination> destination,
            Func<TSource, TDestination> map)
        {
            var i = 0;
            foreach (var destinationItem in destination)
            {
                var sourceItem = source[i++];
                var mappedItem = map(sourceItem);

                var properties = typeof(TDestination).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite).ToList();
                foreach (var property in properties) property.SetValue(destinationItem, property.GetValue(mappedItem));
            }
        }

        public static void Map<TSource, TDestination>(IEnumerable<TSource> source,
            IReadOnlyList<TDestination> destination, Action<TSource, TDestination> map)
        {
            var i = 0;
            foreach (var sourceItem in source)
            {
                var destinationItem = destination[i++];
                map(sourceItem, destinationItem);
            }
        }

        public static IReadOnlyList<ModifiedModel<TModel>> ToModifiedList<TEntity, TModel, TKey>(
            this IReadOnlyList<TModel> models,
            IReadOnlyList<TEntity> originals, Func<TEntity, TModel> mapToModel)
            where TModel : MasterModel<TKey>
            where TKey : IEquatable<TKey>
            where TEntity : Entity<TKey>
        {
            if (models.Count != originals.Count) throw new DbConcurrencyException();

            var modelList = originals.MapReadOnlyList(mapToModel);
            var modelDictionary = modelList.ToDictionary(e => e.Id);

            var result = models.Select(
                model => new ModifiedModel<TModel>
                    {NewValue = model, OriginalValue = modelDictionary[model.Id]}).ToList();

            return result;
        }

        public static IReadOnlyList<TDestination> MapReadOnlyList<TSource, TDestination>(
            this IEnumerable<TSource> source, Action<TSource, TDestination> mapper)
            where TDestination : new()
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceList = source.ToList();
            var destination = new List<TDestination>(sourceList.Count);
            foreach (var sourceItem in sourceList)
            {
                var destinationItem = Factory<TDestination>.New();
                mapper(sourceItem, destinationItem);
                destination.Add(destinationItem);
            }

            return destination.AsReadOnly();
        }

        public static IReadOnlyList<TDestination> MapReadOnlyList<TSource, TDestination>(
            this IEnumerable<TSource> source, Func<TSource, TDestination> mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var sourceList = source.ToList();
            var destination = new List<TDestination>(sourceList.Count);
            foreach (var sourceItem in sourceList)
            {
                var destinationItem = mapper(sourceItem);
                destination.Add(destinationItem);
            }

            return destination.AsReadOnly();
        }
    }
}