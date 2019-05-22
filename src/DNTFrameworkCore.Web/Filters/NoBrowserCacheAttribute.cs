using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Microsoft.Net.Http.Headers;

namespace DNTFrameworkCore.Web.Filters
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
            filterContext.HttpContext.Response.Headers[HeaderNames.CacheControl] =
                         new StringValues(new[] { "no-cache", "max-age=0", "must-revalidate", "no-store" });
            filterContext.HttpContext.Response.Headers[HeaderNames.Expires] = "-1";
            filterContext.HttpContext.Response.Headers[HeaderNames.Pragma] = "no-cache";

            base.OnResultExecuting(filterContext);
        }
    }


   
}