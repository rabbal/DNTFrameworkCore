using System.Collections.Generic;
using System.Linq;
using System.Text;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class ModelStateExtensions
    {
        public static IEnumerable<ValidationFailure> ToFailures(this ModelStateDictionary modelState)
        {
            return modelState.SelectMany(entry =>
            {
                var (key, value) = entry;
                return value.Errors.Select(error => new ValidationFailure(key, error.ErrorMessage));
            });
        }
        
        /// <summary>
        /// Converts the <paramref name="modelState"/> to a dictionary that can be easily serialized.
        /// </summary>
        public static Dictionary<string, string[]> ToSerializable(this ModelStateDictionary modelState)
        {
            return modelState.Where(x => x.Value.Errors.Count > 0).ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray());
        }

        /// <summary>
        /// Stores the errors in a ValidationException object to the specified modelstate dictionary.
        /// </summary>
        /// /// <param name="exception">The validation exception to store</param>
        /// <param name="modelState">The ModelStateDictionary to store the errors in.</param>
        /// <param name="prefix">An optional prefix. If omitted, the property names will be the keys. If specified, the prefix will be concatenated to the property name with a period. Eg "user.Name"</param>
        public static void AddValidationException(this ModelStateDictionary modelState,
            ValidationException exception, string prefix = null)
        {
            if (exception == null) return;

            AddValidation(modelState, exception.Message, exception.Failures, prefix);
        }

        /// <summary>
        /// Stores the errors in a ModelValidationResult object to the specified modelstate dictionary.
        /// </summary>
        /// <param name="result">The validation result to store</param>
        /// <param name="modelState">The ModelStateDictionary to store the errors in.</param>
        /// <param name="prefix">An optional prefix. If omitted, the property names will be the keys. If specified, the prefix will be concatenated to the property name with a period. Eg "user.Name"</param>
        public static void AddResult(this ModelStateDictionary modelState, Result result, string prefix = null)
        {
            if (!result.Failed) return;

            AddValidation(modelState, result.Message, result.Failures, prefix);
        }

        private static void AddValidation(ModelStateDictionary modelState, string message,
            IEnumerable<ValidationFailure> failures, string prefix)
        {
            if (!string.IsNullOrEmpty(message))
            {
                modelState.AddModelError(prefix ?? string.Empty, message);
            }

            foreach (var failure in failures)
            {
                var key = string.IsNullOrEmpty(prefix) || string.IsNullOrEmpty(failure.MemberName)
                    ? failure.MemberName
                    : prefix + "." + failure.MemberName;

                if (!modelState.ContainsKey(key) || modelState[key].Errors.All(i => i.ErrorMessage != failure.Message))
                {
                    modelState.AddModelError(key, failure.Message);
                }
            }
        }

        public static string ToStringFormat(this ModelStateDictionary modelState, bool useHtmlNewLine = false)
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
    }
}