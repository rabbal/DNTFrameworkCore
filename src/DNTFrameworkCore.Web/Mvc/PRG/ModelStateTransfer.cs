using System;
using DNTFrameworkCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.Mvc.PRG
{
    /// <summary>
    /// A base class for Action Filters that need to transfer ModelState to/from TempData
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected const string Key = "ModelStateValue";

        protected static void ExportModelState(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
            {
                controller.TempData[Key] = controller.ModelState.SerializeModelState();
            }
        }

        protected static void ExportModelState(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
            {
                controller.TempData[Key] = controller.ModelState.SerializeModelState();
            }
        }

        protected static void ImportModelState(ActionExecutedContext filterContext)
        {
            if (!(filterContext.Controller is Controller controller) || filterContext.ModelState == null ||
                !controller.TempData.ContainsKey(Key)) return;

            var jsonValue = controller.TempData[Key] as string;

            var modelState = ModelStateExtensions.DeserializeModelState(jsonValue);
            filterContext.ModelState.Merge(modelState);
        }

        protected static void RemoveModelState(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
            {
                controller.TempData.Remove(Key);
            }
        }
    }
}