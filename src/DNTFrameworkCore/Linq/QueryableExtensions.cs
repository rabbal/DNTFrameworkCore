using System;
using System.Linq;
using System.Linq.Expressions;
using DNTFrameworkCore.GuardToolkit;

namespace DNTFrameworkCore.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
            Expression<Func<T, bool>> predicate)
        {
            Guard.ArgumentNotNull(query, nameof(query));

            return condition
                ? query.Where(predicate)
                : query;
        }
    }
}