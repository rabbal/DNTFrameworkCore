using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Querying;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.EFCore.Querying
{
    public static class QueryableExtensions
    {
        public static Task<IPagedResult<T>> ToPagedListAsync<T>(this IQueryable<T> query, IPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return query.ToPagedListAsync(request.Page, request.PageSize, request.ParsedSorting, null,
                cancellationToken);
        }

        public static Task<IPagedResult<T>> ToPagedListAsync<T>(this IQueryable<T> query, IFilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return query.ToPagedListAsync(request.Page, request.PageSize, request.ParsedSorting,
                request.ParsedFiltering, cancellationToken);
        }

        public static async Task<IPagedResult<T>> ToPagedListAsync<T>(this IQueryable<T> query,
            int page,
            int pageSize,
            IEnumerable<SortExpression> sorts,
            IEnumerable<FilterExpression> filters,
            CancellationToken cancellationToken = default)
        {
            if (filters != null) query = query.Filter(filters);

            var totalCount = await query.LongCountAsync(cancellationToken);
            
            query = query.Sort(sorts);
            query = query.Page(page, pageSize);

            return new PagedResult<T>
            {
                ItemList = await query.ToListAsync(cancellationToken),
                TotalCount = totalCount
            };
        }
    }
}