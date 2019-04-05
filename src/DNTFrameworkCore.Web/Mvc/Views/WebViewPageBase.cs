using System.Globalization;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Runtime;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Razor.Internal;

namespace DNTFrameworkCore.Web.Mvc.Views
{
    public abstract class WebViewPageBase<TModel> : RazorPage<TModel>
    {
        [RazorInject] public IHtmlLocalizerFactory HtmlLocalizerFactory { get; set; }
        [RazorInject] public IUserSession UserSession { get; set; }

        private IHtmlLocalizer HtmlLocalizer =>
            HtmlLocalizerFactory.Create(LocalizationResourceName, LocalizationResourceLocation);

        protected bool IsAuthenticated => UserSession.IsAuthenticated;

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

        protected string BuildMenuUrl(string url)
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
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
        /// </summary>
        protected string LocalizationResourceName { get; set; } = "SharedResource";

        /// <summary>
        /// The location to load resources from
        /// It must be set in order to use <see cref="L(string)"/> and <see cref="L(string,CultureInfo)"/> methods.
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

        /// <summary>
        /// Gets localized string for given key name and specified culture information.
        /// </summary>
        protected string L(string name, CultureInfo culture) => HtmlLocalizer.WithCulture(culture).GetString(name);

        /// <summary>
        /// Gets localized string for given key name and current language with formatting strings.
        /// </summary>
        protected string L(string name, CultureInfo culture, params object[] args) =>
            HtmlLocalizer.WithCulture(culture).GetString(name, args);

        /// <summary>
        /// Checks if current user is granted for a permission.
        /// </summary>
        /// <param name="permissionName">Name of the permission</param>
        protected bool IsGranted(string permissionName)
        {
            return UserSession.IsGranted(permissionName);
        }
    }
}