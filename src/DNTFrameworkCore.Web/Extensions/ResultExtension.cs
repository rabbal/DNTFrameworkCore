using System.Linq;
using DNTFrameworkCore.Functional;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class ResultExtension
    {
        /// <summary>
        /// Stores the errors in a ValidationResult object to the specified modelstate dictionary.
        /// </summary>
        /// <param name="result">The validation result to store</param>
        /// <param name="modelState">The ModelStateDictionary to store the errors in.</param>
        /// <param name="prefix">An optional prefix. If ommitted, the property names will be the keys. If specified, the prefix will be concatenatd to the property name with a period. Eg "user.Name"</param>
        public static void AddToModelState(this Result result, ModelStateDictionary modelState, string prefix = null)
        {
            if (result.Succeeded) return;

            foreach (var failure in result.Failures)
            {
                var key = string.IsNullOrEmpty(prefix) ? failure.MemberName : prefix + "." + failure.MemberName;
                if (!modelState.ContainsKey(failure.MemberName) ||
                    modelState[failure.MemberName].Errors.All(i => i.ErrorMessage != failure.Message))
                {
                    modelState.AddModelError(failure.MemberName, failure.Message);
                }
            }

        }
    }
}