using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Reflection;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.Querying
{
    public static class QueryableExtensions
    {
        private static readonly ConcurrentDictionary<Type, PropertyInfo[]> _properties = new();

        public static IQueryable<T> Filter<T>(this IQueryable<T> query, IEnumerable<FilterExpression> filters)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (filters == null) throw new ArgumentNullException(nameof(filters));

            //todo:return query.Where(filters.ToLambdaExpression<T>());
            return query;
        }

        /// <summary>
        /// Applies sorting over IQueryable using Dynamic Linq.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="sorts"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<T> Sort<T>(this IQueryable<T> query, IEnumerable<SortExpression> sorts)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (sorts == null) throw new ArgumentNullException(nameof(sorts));

            var ordering = string.Join(",", sorts);
            if (string.IsNullOrEmpty(ordering)) ordering = GetDefaultSorting(typeof(T)).ToString();

            return query.OrderBy(ordering);
        }

        /// <summary>
        /// Applies paging over IQueryable using Dynamic Linq.
        /// </summary>
        /// <param name="query">The IQueryable which should be processed.</param>
        /// <param name="page">Specify the pageIndex</param>
        /// <param name="pageSize"></param>
        /// <typeparam name="T">The type of IQueryable.</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static IQueryable<T> Page<T>(this IQueryable<T> query, int page, int pageSize)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            var skip = (page - 1) * pageSize;
            var take = pageSize;

            return query.Skip(skip).Take(take);
        }

        private static SortExpression GetDefaultSorting(Type type)
        {
            var properties =
                _properties.GetOrAdd(type, t => t.GetProperties(BindingFlags.Instance | BindingFlags.Public));

            var property =
                properties.FirstOrDefault(p => string.Equals(p.Name, "id", StringComparison.OrdinalIgnoreCase));
            if (property == null)
                property = properties.FirstOrDefault(p => p.PropertyType.IsPredefinedType()) ??
                           throw new NotSupportedException("There is not any public property of primitive type for sorting");

            return new SortExpression(property.Name, true);
        }
    }
}