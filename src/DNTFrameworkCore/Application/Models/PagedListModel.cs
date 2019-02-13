namespace DNTFrameworkCore.Application.Models
{
    public class PagedListModel<TModel, TPagedQueryModel> where TPagedQueryModel : IPagedQueryModel
    {
        public TPagedQueryModel Query { get; set; }

        public IPagedQueryResult<TModel> Result { get; set; }
    }
}