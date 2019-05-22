using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using static DNTFrameworkCore.Web.Extensions.ModelStateExtensions;

namespace DNTFrameworkCore.Web.Filters
{
    /// <summary>
    /// A base class for Action Filters that need to transfer ModelState to/from TempData
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected static readonly string Key = typeof(ModelStateTransfer).FullName;

        protected static void ExportModelStateToTempData(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            filterContext.ModelState.ExportModelStateToTempData(controller, Key);
        }

        protected static void ExportModelStateToTempData(ActionExecutingContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            filterContext.ModelState.ExportModelStateToTempData(controller, Key);
        }

        protected static void ImportModelStateFromTempData(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null && filterContext.ModelState != null && controller.TempData.ContainsKey(Key))
            {
                var json = controller.TempData[Key] as string;
                var modelState = DeserializeModelState(json);
                filterContext.ModelState.Merge(modelState);
            }
        }
        protected static void RemoveModelStateFromTempData(ActionExecutedContext filterContext)
        {
            var controller = filterContext.Controller as Controller;
            if (controller != null)
            {
                controller.TempData.Remove(Key);
            }
        }
        private static ModelStateDictionary DeserializeModelState(string json)
        {
            var values = JsonConvert.DeserializeObject<List<ModelStateTransferValue>>(json);
            var modelState = new ModelStateDictionary();

            foreach (var item in values)
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (var error in item.ErrorMessages)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }
            return modelState;
        }
    }
}