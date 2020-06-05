using DNTFrameworkCore.Extensions;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace DNTFrameworkCore.Web.Mvc.ViewEngine
{
    public abstract class WebViewPageBase<TModel> : RazorPage<TModel>
    {
        [RazorInject] public IHtmlLocalizerFactory HtmlLocalizerFactory { get; set; }

        private IHtmlLocalizer HtmlLocalizer =>
            HtmlLocalizerFactory.Create(LocalizationResourceName, LocalizationResourceLocation);

        protected bool IsAuthenticated => Context.User.Identity.IsAuthenticated;

        protected string ApplicationPath
        {
            get
            {
                var appPath = Context.Request.PathBase.Value;
                if (appPath == null)
                {
                    return "/";
                }

                appPath = appPath.EnsureEndsWith('/');

                return appPath;
            }
        }

        protected string MenuUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return ApplicationPath;
            }

            if (UrlChecker.IsRooted(url))
            {
                return url;
            }

            return ApplicationPath + url;
        }

        /// <summary>
        /// The name of the resource to load strings from
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string)"/> methods.
        /// </summary>
        protected string LocalizationResourceName { get; set; } = "SharedResource";

        /// <summary>
        /// The location to load resources from
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string)"/> methods.
        /// </summary>
        protected string LocalizationResourceLocation { get; set; }

        /// <summary>
        /// Gets localized string for given key name and current language.
        /// </summary>
        protected string L(string name) => HtmlLocalizer.GetString(name);

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        protected string L(string name, params object[] args) => HtmlLocalizer.GetString(name, args);
    }
}