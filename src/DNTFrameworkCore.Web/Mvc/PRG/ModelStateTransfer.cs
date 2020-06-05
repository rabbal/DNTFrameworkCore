using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNTFrameworkCore.Web.Mvc.PRG
{
    /// <summary>
    /// A base class for Action Filters that need to transfer ModelState to/from TempData
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public abstract class ModelStateTransfer : ActionFilterAttribute
    {
        protected static void ExportModelState(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
            {
                controller.TempData[nameof(Controller.ModelState)] = SerializeModelState(controller.ModelState);
            }
        }

        protected static void ExportModelState(ActionExecutingContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
            {
                controller.TempData[nameof(Controller.ModelState)] = SerializeModelState(controller.ModelState);
            }
        }

        protected static void ImportModelState(ActionExecutedContext filterContext)
        {
            if (!(filterContext.Controller is Controller controller) ||
                !controller.TempData.ContainsKey(nameof(Controller.ModelState))) return;

            var jsonValue = controller.TempData[nameof(Controller.ModelState)] as string;

            var modelState = DeserializeModelState(jsonValue);
            filterContext.ModelState.Merge(modelState);
        }

        protected static void RemoveModelState(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller is Controller controller)
            {
                controller.TempData.Remove(nameof(Controller.ModelState));
            }
        }

        private static string SerializeModelState(ModelStateDictionary modelState)
        {
            var values = modelState
                .Select(kvp => new ModelStateItemValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    Errors = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                });

            return JsonSerializer.Serialize(values);
        }

        private static ModelStateDictionary DeserializeModelState(string jsonValue)
        {
            var itemList = JsonSerializer.Deserialize<List<ModelStateItemValue>>(jsonValue);
            var modelState = new ModelStateDictionary();

            foreach (var item in itemList)
            {
                modelState.SetModelValue(item.Key, item.RawValue, item.AttemptedValue);
                foreach (var error in item.Errors)
                {
                    modelState.AddModelError(item.Key, error);
                }
            }

            return modelState;
        }

        private class ModelStateItemValue
        {
            public string Key { get; set; }
            public string AttemptedValue { get; set; }
            public object RawValue { get; set; }
            public ICollection<string> Errors { get; set; } = new List<string>();
        }
    }
}