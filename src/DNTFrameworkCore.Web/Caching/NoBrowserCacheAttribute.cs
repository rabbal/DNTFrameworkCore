using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.Caching
{
    /// <summary>
    /// Sets `no-cache`, `must-revalidate`, `no-store` headers for the current `Response`.
    /// </summary>
    public class NoBrowserCacheAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// surrounds execution of the action
        /// </summary>
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.DisableBrowserCache();
            base.OnResultExecuting(filterContext);
        }
    }
}