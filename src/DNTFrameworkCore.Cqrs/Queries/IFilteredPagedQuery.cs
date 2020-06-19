using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IFilteredPagedQuery<out TReadModel> : IQuery<IPagedResult<TReadModel>>, IFilteredPagedRequest
    {
    }

    public class FilteredPagedQuery<TReadModel> : FilteredPagedRequest, IFilteredPagedQuery<TReadModel>
    {
    }
}