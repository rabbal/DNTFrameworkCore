using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DNTFrameworkCore.GuardToolkit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;

namespace DNTFrameworkCore.Web.Mvc.HtmlHelpers
{
    public static class HtmlPrefixScopeExtensions
    {
        private const string JQueryTemplatingEnabledKey = "__BeginCollectionItem_jQuery";

        /// <summary>
        /// Begins a collection item by inserting either a previously used .Index hidden field value for it or a new one.
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="collectionName">The name of the collection property from the Model that owns this item.</param>
        /// <returns></returns>
        public static IDisposable BeginCollectionItem<TModel>(this IHtmlHelper<TModel> htmlHelper, string collectionName)
        {
            Guard.ArgumentNotEmpty(collectionName, nameof(collectionName));

            var htmlFieldPrefix = htmlHelper.ViewData.TemplateInfo.HtmlFieldPrefix;
            if (htmlFieldPrefix.Contains(collectionName))
            {
                collectionName = htmlFieldPrefix.Substring(0, htmlFieldPrefix.LastIndexOf(collectionName, StringComparison.Ordinal) + collectionName.Length);
            }

            var collectionIndexFieldName = $"{collectionName}.Index";

            string itemIndex = null;
            if (htmlHelper.ViewData.ContainsKey(JQueryTemplatingEnabledKey))
            {
                itemIndex = "${index}";
            }
            else
            {
                itemIndex = BuildCollectionItemIndex(htmlHelper.ViewContext.HttpContext, collectionIndexFieldName);
            }

            htmlHelper.ViewContext.Writer.WriteLine(
                $"<input type=\"hidden\" name=\"{collectionIndexFieldName}\" autocomplete=\"off\" value=\"{htmlHelper.Encode(itemIndex)}\" />");

            var collectionItemName = $"{collectionName}[{itemIndex}]";

            return BeginHtmlFieldPrefixScope(htmlHelper, collectionItemName);
        }

        public static async Task CollectionItemJQueryTemplateAsync<TModel, TCollectionItem>(
            this HtmlHelper<TModel> htmlHelper,
            string partialViewName,
            TCollectionItem model)
        {
            ViewDataDictionary<TCollectionItem> viewData = new ViewDataDictionary<TCollectionItem>(htmlHelper.ViewData, model);
            viewData.Add(JQueryTemplatingEnabledKey, true);
            await htmlHelper.RenderPartialAsync(partialViewName, model, viewData);
        }

        public static async Task RenderPartialCollectionAsync<TModel, TProperty>(this IHtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, string partialViewName)
          where TProperty : IEnumerable
        {
            var metadata = ExpressionMetadataProvider.FromLambdaExpression(expression, htmlHelper.ViewData, htmlHelper.MetadataProvider);
            var items = (IEnumerable)metadata.Model ?? Enumerable.Empty<TModel>();
            foreach (var item in items)
            {
                using (htmlHelper.BeginCollectionItem(metadata.Metadata.PropertyName))
                {
                    await htmlHelper.RenderPartialAsync(partialViewName, item);
                }
            }
        }

        /// <summary>
        /// Tries to reuse old .Index values from the HttpRequest in order to keep the ModelState consistent
        /// across requests. If none are left returns a new one.
        /// </summary>
        /// <param name="collectionIndexFieldName"></param>
        /// <returns>a GUID string</returns>
        private static string BuildCollectionItemIndex(HttpContext httpContext, string collectionIndexFieldName)
        {
            Queue<string> previousIndexes = (Queue<string>)httpContext.Items[collectionIndexFieldName];
            if (previousIndexes == null)
            {
                httpContext.Items[collectionIndexFieldName] = previousIndexes = new Queue<string>();

                if (httpContext.Request.HasFormContentType)
                {
                    string previousIndexValues = httpContext.Request.Form[collectionIndexFieldName];
                    if (!String.IsNullOrWhiteSpace(previousIndexValues))
                    {
                        foreach (string index in previousIndexValues.Split(','))
                        {
                            previousIndexes.Enqueue(index);
                        }
                    }
                }
            }

            return previousIndexes.Count > 0 ? previousIndexes.Dequeue() : Guid.NewGuid().ToString();
        }

        public static IDisposable BeginHtmlFieldPrefixScope(this IHtmlHelper htmlHelper, string htmlFieldPrefix)
        {
            return new HtmlFieldPrefixScope(htmlHelper.ViewData.TemplateInfo, htmlFieldPrefix);
        }

        private class HtmlFieldPrefixScope : IDisposable
        {
            private readonly TemplateInfo _templateInfo;
            private readonly string _previousPrefix;

            public HtmlFieldPrefixScope(TemplateInfo templateInfo, string htmlFieldPrefix)
            {
                _templateInfo = templateInfo;

                _previousPrefix = templateInfo.HtmlFieldPrefix;
                templateInfo.HtmlFieldPrefix = htmlFieldPrefix;
            }

            public void Dispose()
            {
                _templateInfo.HtmlFieldPrefix = _previousPrefix;
            }
        }
    }
}