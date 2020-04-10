using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IFilteredPagedQuery<out TReadModel> : IPagedQuery<TReadModel>, IFilteredPagedRequest
    {
    }

    public class FilteredPagedQuery<TReadModel> : PagedQuery<TReadModel>, IFilteredPagedQuery<TReadModel>
    {
        public FilteringCriteria Filtering { get; set; }
    }
}