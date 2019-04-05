using DNTFrameworkCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Routing;

namespace DNTFrameworkCore.Web.Mvc
{
    /// <summary>
    /// Determines whether the HttpRequest's X-Requested-With header has XMLHttpRequest value.
    /// </summary>
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        /// <summary>
        /// Determines whether the action selection is valid for the specified route context.
        /// </summary>
        public override bool IsValidForRequest(RouteContext routeContext, ActionDescriptor action)
        {
            return routeContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}