using System.Collections.Generic;

namespace DNTFrameworkCore.Querying
{
    public interface IPagedResult<out TModel>
    {
        IReadOnlyList<TModel> ItemList { get; }
        long TotalCount { get; }
    }
    
    public class PagedResult<TModel> : IPagedResult<TModel>
    {
        public IReadOnlyList<TModel> ItemList { get; set; } = new List<TModel>();
        public long TotalCount { get; set; }
    }
}