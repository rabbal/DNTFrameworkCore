using System.Collections.Generic;

namespace DNTFrameworkCore.Application.Models
{
    public interface IPagedQueryResult<out TModel>
    {
        IReadOnlyList<TModel> Items { get; }
        long TotalCount { get; }
    }

    public class PagedQueryResult<TModel> : IPagedQueryResult<TModel>
    {
        public IReadOnlyList<TModel> Items { get; set; } = new List<TModel>();
        public long TotalCount { get; set; }
    }
}