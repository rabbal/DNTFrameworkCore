using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

namespace DNTFrameworkCore.Querying
{
    public static class QueryableExtensions
    {
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

        private static Expression<Func<TElement, bool>> ToLambdaExpression<TElement>(
            this FilterExpression filterExpression)
        {
            var flattenList = filterExpression.ToFlatList();
            var predicate = filterExpression.ToExpression((item) => flattenList.IndexOf(item));
            var values = flattenList.Select(f => f.Value).ToArray();

            return DynamicExpressionParser.ParseLambda<TElement, bool>(new ParsingConfig(), false, predicate, values);
        }

        /// <summary>
        /// Converts the filter expression to a predicate suitable for Dynamic Linq e.g. "Field1 = @1 and Field2.Contains(@2)"
        /// </summary>
        private static string ToExpression(this FilterExpression filterExpression,
            Func<FilterExpression, int> indexFactory)
        {
            if (filterExpression.Filters != null && filterExpression.Filters.Any())
            {
                return "(" + string.Join(" " + filterExpression.Logic + " ",
                           filterExpression.Filters.Select(filter => filter.ToExpression(indexFactory)).ToArray()) +
                       ")";
            }

            var index = indexFactory(filterExpression);

            var comparison = _operators[filterExpression.Operator];

            if (filterExpression.Operator == "doesnotcontain")
            {
                return $"!{filterExpression.Field}.{comparison}(@{index})";
            }

            if (comparison == "StartsWith" || comparison == "EndsWith" || comparison == "Contains")
            {
                return $"{filterExpression.Field}.{comparison}(@{index})";
            }

            return $"{filterExpression.Field} {comparison} @{index}";
        }

        private static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
        {
            {"eq", "="},
            {"neq", "!="},
            {"lt", "<"},
            {"lte", "<="},
            {"gt", ">"},
            {"gte", ">="},
            {"startswith", "StartsWith"},
            {"endswith", "EndsWith"},
            {"contains", "Contains"},
            {"doesnotcontain", "Contains"}
        };
    }
}