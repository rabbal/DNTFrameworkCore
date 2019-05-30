using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Mapping
{
    public static class MappingExtensions
    {
        public static IReadOnlyList<TDestination> MapReadOnlyList<TSource, TDestination>(this IEnumerable<TSource> source, Action<TSource, TDestination> mapper)
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

            var destination = new List<TDestination>(source.Count());
            foreach (var sourceItem in source)
            {
                var destinationItem = Factory<TDestination>.CreateInstance();
                mapper(sourceItem, destinationItem);
                destination.Add(destinationItem);
            }

            return destination.AsReadOnly();
        }

        public static IReadOnlyList<TDestination> MapReadOnlyList<TSource, TDestination>(this IEnumerable<TSource> source, Func<TSource, TDestination> mapper)
        {
            if (mapper == null)
            {
                throw new ArgumentNullException(nameof(mapper));
            }

            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var destination = new List<TDestination>(source.Count());
            foreach (var sourceItem in source)
            {
                var destinationItem = mapper(sourceItem);
                destination.Add(destinationItem);
            }

            return destination.AsReadOnly();
        }
    }
}
