using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNTFrameworkCore.Functional;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class ModelStateExtensions
    {
        /// <summary>
        /// Converts the <paramref name="modelState"/> to a dictionary that can be easily serialized.
        /// </summary>
        public static IDictionary<string, string[]> ToSerializableDictionary(this ModelStateDictionary modelState)
        {
            return modelState.Where(x => x.Value.Errors.Any()).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );
        }

        /// <summary>
        /// Stores the errors in a ModelValidationResult object to the specified modelstate dictionary.
        /// </summary>
        /// <param name="result">The validation result to store</param>
        /// <param name="modelState">The ModelStateDictionary to store the errors in.</param>
        /// <param name="prefix">An optional prefix. If ommitted, the property names will be the keys. If specified, the prefix will be concatenatd to the property name with a period. Eg "user.Name"</param>
        public static void AddModelError(this ModelStateDictionary modelState, Result result, string prefix = null)
        {
            if (!result.Failed) return;

            foreach (var failure in result.Failures)
            {
                var key = string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(failure.MemberName) ? failure.MemberName : prefix + "." + failure.MemberName;
                if (!modelState.ContainsKey(key) ||
                    modelState[key].Errors.All(i => i.ErrorMessage != failure.Message))
                {
                    modelState.AddModelError(key, failure.Message);
                }
            }

        }
        public static string ExportErrors(this ModelStateDictionary modelState, bool useHtmlNewLine = false)
        {
            var builder = new StringBuilder();

            foreach (var error in modelState.Values.SelectMany(a => a.Errors))
            {
                var message = error.ErrorMessage;
                if (string.IsNullOrWhiteSpace(message))
                {
                    continue;
                }

                builder.AppendLine(!useHtmlNewLine ? message : $"{message}<br/>");
            }

            return builder.ToString();
        }


        public static void ExportModelStateToTempData(this ModelStateDictionary modelState, Controller controller, string key)
        {
            if (controller != null && modelState != null)
            {
                var modelStateJson = SerializeModelState(modelState);
                controller.TempData[key] = modelStateJson;
            }
        }

        public static string SerializeModelState(this ModelStateDictionary modelState)
        {
            var values = modelState
                .Select(kvp => new ModelStateTransferValue
                {
                    Key = kvp.Key,
                    AttemptedValue = kvp.Value.AttemptedValue,
                    RawValue = kvp.Value.RawValue,
                    ErrorMessages = kvp.Value.Errors.Select(err => err.ErrorMessage).ToList(),
                });

            return JsonConvert.SerializeObject(values);
        }

        public class ModelStateTransferValue
        {
            public string Key { get; set; }
            public string AttemptedValue { get; set; }
            public object RawValue { get; set; }
            public ICollection<string> ErrorMessages { get; set; } = new List<string>();
        }
    }
}