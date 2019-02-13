using System.Threading.Tasks;
using DNTFrameworkCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.Web.Results
{
    /// <summary>
    /// An AJAX aware version of <see cref="Microsoft.AspNetCore.Mvc.RedirectResult"/>
    /// </summary>
    public class AjaxAwareRedirectResult : RedirectResult
    {
        public AjaxAwareRedirectResult(string url, bool permanent = false) : base(url, permanent)
        {
        }
        
        public override async Task ExecuteResultAsync(ActionContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                var redirectUrl = UrlHelper.Content(Url);
                var javaScriptRedirectResult = new JavaScriptRedirectResult(redirectUrl);

                await javaScriptRedirectResult.ExecuteResultAsync(context);
            }
            else
            {
                await base.ExecuteResultAsync(context);
            }
        }
    }
}