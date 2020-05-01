using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.Mvc.PRG
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class ImportModelStateAttribute : ModelStateTransfer
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!(filterContext.Controller is Controller controller) || filterContext.ModelState == null) return;

            var jsonValue = controller.TempData[Key] as string;

            if (string.IsNullOrEmpty(jsonValue)) return;

            if (filterContext.Result is ViewResult)
            {
                ImportModelState(filterContext);
            }
            else
            {
                RemoveModelState(filterContext);
            }

            base.OnActionExecuted(filterContext);
        }
    }
}