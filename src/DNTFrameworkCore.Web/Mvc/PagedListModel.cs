using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Web.Mvc
{
    public class PagedListModel<TModel, TPagedRequestModel> where TPagedRequestModel : IPagedRequest
    {
        public TPagedRequestModel Request { get; set; }
        public IPagedResult<TModel> Result { get; set; }
    }
}