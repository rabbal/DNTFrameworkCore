using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IPagedQuery<out TReadModel> : IQuery<IPagedResult<TReadModel>>, IPagedRequest
    {
    }

    public class PagedQuery<TReadModel> : IPagedQuery<TReadModel>
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public string SortExpression { get; set; }
    }
}