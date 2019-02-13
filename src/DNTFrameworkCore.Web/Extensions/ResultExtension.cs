using System.Linq;
using DNTFrameworkCore.Functional;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class ResultExtension
    {
        public static void AddToModelState(this Result result, ModelStateDictionary modelState)
        {
            if (result.Succeeded) return;

            modelState.AddModelError(string.Empty, result.Message);

            foreach (var failure in result.Failures)
            {
                if (!modelState.ContainsKey(failure.MemberName) ||
                    modelState[failure.MemberName].Errors.All(i => i.ErrorMessage != failure.Message))
                {
                    modelState.AddModelError(failure.MemberName, failure.Message);
                }
            }
        }
    }
}