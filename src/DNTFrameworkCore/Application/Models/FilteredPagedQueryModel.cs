namespace DNTFrameworkCore.Application.Models
{
    public interface IFilteredPagedQueryModel : IPagedQueryModel
    {
        Filter Filter { get; set; }
    }
    
    public class FilteredPagedQueryModel : PagedQueryModel, IFilteredPagedQueryModel
    {
        public Filter Filter { get; set; }
    }
}