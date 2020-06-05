using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.Web.Mvc
{
    public class PagedListModel<TModel, TPagedRequest> where TPagedRequest : IPagedRequest
    {
        public TPagedRequest Request { get; set; }
        public IPagedResult<TModel> Result { get; set; }
    }
}