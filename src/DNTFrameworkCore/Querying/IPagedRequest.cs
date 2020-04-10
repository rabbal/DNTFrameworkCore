namespace DNTFrameworkCore.Querying
{
    public interface IPagedRequest
    {
        int Page { get; set; }
        int PageSize { get; set; }
        string SortExpression { get; set; }
    }
}