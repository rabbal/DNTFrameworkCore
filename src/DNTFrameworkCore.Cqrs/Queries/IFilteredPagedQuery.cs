using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IFilteredPagedQuery<out TReadModel> : IPagedQuery<TReadModel>
    {
        Filter Filter { get; set; }
    }

    public class FilteredPagedQuery<TReadModel> : PagedQuery<TReadModel>, IFilteredPagedQuery<TReadModel>
    {
        public Filter Filter { get; set; }
    }
}