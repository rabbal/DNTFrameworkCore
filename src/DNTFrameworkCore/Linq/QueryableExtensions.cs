using System;
using System.Linq;
using System.Linq.Expressions;

namespace DNTFrameworkCore.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition,
            Expression<Func<T, bool>> predicate)
        {
            if (query == null) throw new ArgumentNullException(nameof(query));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            return condition
                ? query.Where(predicate)
                : query;
        }
    }
}