using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.GuardToolkit;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EntityFramework.Context.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, IFilteredPagedQueryModel model)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotNull(model, nameof(model));

            return query.ApplyFiltering(model.Filter);
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, IPagedQueryModel model)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotNull(model, nameof(model));

            return query.ApplySorting(model.SortExpression);
        }

        public static IQueryable<T> ApplyPaging<T>(this IQueryable<T> query, IPagedQueryModel model)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotNull(model, nameof(model));

            if (model.Page < 1) model.Page = 1;

            if (model.PageSize < 1) model.PageSize = 10;

            return query.ApplyPaging(model.Page, model.PageSize);
        }

        public static Task<PagedQueryResult<T>> ToPagedQueryResultAsync<T>(this IQueryable<T> query,
            IPagedQueryModel model)
        {
            var filter = model is IFilteredPagedQueryModel filteredQuery ? filteredQuery.Filter : null;

            return query.ToPagedQueryResultAsync(model.Page, model.PageSize, model.SortExpression, filter);
        }

        public static Task<PagedQueryResult<T>> ToPagedQueryResultAsync<T>(this IQueryable<T> query,
            int page,
            int pageSize,
            string sortExpression)
        {
            return query.ToPagedQueryResultAsync(page, pageSize, sortExpression, null);
        }

        public static async Task<PagedQueryResult<T>> ToPagedQueryResultAsync<T>(this IQueryable<T> query,
            int page,
            int pageSize,
            string sortExpression,
            Filter filter)
        {
            query = query.ApplyFiltering(filter);

            var totalCount = await query.LongCountAsync();

            query = query.ApplySorting(sortExpression);
            query = query.ApplyPaging(page, pageSize);

            return new PagedQueryResult<T>
            {
                Items = await query.ToListAsync(),
                TotalCount = totalCount
            };
        }

        public static IQueryable<T> ApplyFiltering<T>(this IQueryable<T> query, Filter filter)
        {
            Guard.ArgumentNotNull(query, nameof(query));

            if (filter?.Logic == null) return query;

            var filters = filter.List();

            // Get all filter values as array (needed by the Where method of Dynamic Linq)
            var values = filters.Select(f => f.Value).ToArray();

            // Create a predicate expression e.g. Field1 = @0 And Field2 > @1
            var predicate = filter.ToExpression(filters);

            // Use the Where method of Dynamic Linq to filter the data
            query = query.Where(predicate, values);

            return query;
        }

        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string sortExpression)
        {
            Guard.ArgumentNotNull(query, nameof(query));
            Guard.ArgumentNotEmpty(sortExpression, nameof(sortExpression));

            return query.OrderBy(sortExpression.Replace('_', ' '));
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