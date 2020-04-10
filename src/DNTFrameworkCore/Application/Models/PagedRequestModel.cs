using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Application.Models
{
    public class FilteredPagedRequestModel : PagedRequestModel, IFilteredPagedRequest
    {
        public FilteringCriteria Filtering { get; set; }
    }

    public class PagedRequestModel : IPagedRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortExpression { get; set; } = "Id_DESC";
    }
}