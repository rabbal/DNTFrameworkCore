using System.Globalization;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.Localization
{
    public class LanguageAttribute : ActionFilterAttribute
    {
        private readonly string _cultureName;

        public LanguageAttribute(string cultureName)
        {
            _cultureName = cultureName;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            CultureInfo.CurrentCulture = new CultureInfo(_cultureName);
            CultureInfo.CurrentUICulture = new CultureInfo(_cultureName);

            base.OnActionExecuting(context);
        }
    }
}