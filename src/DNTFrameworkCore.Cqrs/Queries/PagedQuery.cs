namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IPagedQuery : IQuery
    {
        int Page { get; set; }
        int PageSize { get; set; }
        string SortExpression { get; set; }
    }

    public class PagedQuery : IPagedQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SortExpression { get; set; } = "Id_DESC";
    }
}