using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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

        public static string DumpErrors(this ModelStateDictionary modelState, bool useHtmlNewLine = false)
        {
            var results = new StringBuilder();

            foreach (var error in modelState.Values.SelectMany(a => a.Errors))
            {
                var errorDescription = error.ErrorMessage;
                if (string.IsNullOrWhiteSpace(errorDescription))
                {
                    continue;
                }

                results.AppendLine(!useHtmlNewLine ? errorDescription : $"{errorDescription}<br/>");
            }

            return results.ToString();
        }
    }
}