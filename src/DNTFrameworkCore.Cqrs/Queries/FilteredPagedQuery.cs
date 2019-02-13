using DNTFrameworkCore.Application.Models;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IFilteredPagedQuery : IPagedQuery
    {
        Filter Filter { get; set; }
    }

    public class FilteredPagedQuery : PagedQuery, IFilteredPagedQuery
    {
        public Filter Filter { get; set; }
    }
}