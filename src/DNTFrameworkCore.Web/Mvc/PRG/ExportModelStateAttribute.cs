using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.Mvc.PRG
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ExportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.ModelState.IsValid)
            {
                if (filterContext.Result is RedirectResult
                    || filterContext.Result is RedirectToRouteResult
                    || filterContext.Result is RedirectToActionResult)
                {
                    if (filterContext.Controller is Controller controller && filterContext.ModelState != null)
                    {
                        ExportModelState(filterContext);
                    }
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}