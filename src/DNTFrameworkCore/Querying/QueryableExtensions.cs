using System.Linq;
using System.Linq.Dynamic.Core;
using DNTFrameworkCore.GuardToolkit;

namespace DNTFrameworkCore.Querying
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, IFilteredPagedRequest request)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotNull(request, nameof(request));

            return query.ApplyFiltering(request.Filtering);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, IPagedRequest request)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotNull(request, nameof(request));

            return query.ApplySorting(request.SortExpression);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, IPagedRequest request)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotNull(request, nameof(request));

            if (request.Page < 1) request.Page = 1;

            if (request.PageSize < 1) request.PageSize = 10;

            return query.ApplyPaging(request.Page, request.PageSize);
        }

        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, FilteringCriteria filtering)
        {
            Guard.ArgumentNotNull(query, nameof(query));

            if (filtering?.Logic == null) return query;

            var filters = filtering.ToFlatList();

            // Get all filter values as array (needed by the Where method of Dynamic Linq)
            var values = filters.Select(f => f.Value).ToArray();

            // Create a predicate expression e.g. Field1 = @0 And Field2 > @1
            var predicate = filtering.ToExpression(filters);

            // Use the Where method of Dynamic Linq to filter the data
            query = query.Where(predicate, values);

            return query;
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string expression)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotEmpty(expression, nameof(expression));

            return query.OrderBy(expression.Replace('_', ' '));
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, int page, int pageSize)
        {
            Guard.ArgumentNotNull(query, nameof(query));

            var skip = (page - 1) * pageSize;
            var take = pageSize;

            return query.Skip(skip).Take(take);
        }
    }
}