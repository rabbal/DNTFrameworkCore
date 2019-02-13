using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Html;

namespace DNTFrameworkCore.Web.Mvc
{
    /// <summary>
    /// Html Helper Extensions
    /// </summary>
    public static class HtmlHelperExtensions
    {
        /// <summary>
        /// Convert IHtmlContent/TagBuilder to string
        /// </summary>
        public static string GetString(this IHtmlContent content)
        {
            using (var writer = new StringWriter())
            {
                content.WriteTo(writer, HtmlEncoder.Default);
                return writer.ToString();
            }
        }
    }
}