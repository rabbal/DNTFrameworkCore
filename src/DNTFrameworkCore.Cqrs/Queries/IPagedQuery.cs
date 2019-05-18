using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IPagedQuery<out TReadModel> : IQuery<IPagedQueryResult<TReadModel>>
    {
        int Page { get; set; }
        int PageSize { get; set; }
        string SortExpression { get; set; }
    }

    public class PagedQuery<TReadModel> : IPagedQuery<TReadModel>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortExpression { get; set; }
    }
}