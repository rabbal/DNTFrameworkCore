namespace DNTFrameworkCore.Application.Models
{
    public class FilteredPagedListModel<TModel, TFilteredPagedQueryModel>
        where TFilteredPagedQueryModel : IFilteredPagedQueryModel
    {
        public TFilteredPagedQueryModel Query { get; set; }

        public PagedQueryResult<TModel> Result { get; set; }
    }
}