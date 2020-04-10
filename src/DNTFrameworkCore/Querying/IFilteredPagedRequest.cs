namespace DNTFrameworkCore.Querying
{
    public interface IFilteredPagedRequest : IPagedRequest
    {
        FilteringCriteria Filtering { get; set; }
    }
}