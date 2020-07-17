using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DNTFrameworkCore.EFCore.Linq
{
    public static class QueryableExtensions
    {
        public static IQueryable<TEntity> IncludePaths<TEntity>(this IQueryable<TEntity> source,
            params string[] paths) where TEntity : class
        {
            return !(source.Provider is EntityQueryProvider)
                ? source
                : source.Include(string.Join(".", paths));
        }
    }
}