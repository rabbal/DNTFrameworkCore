using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public class FormValueExistsAttribute : ActionFilterAttribute
    {
        private readonly string _name;
        private readonly string _value;
        private readonly string _actionParameterName;

        public FormValueExistsAttribute(string name, string value, string actionParameterName)
        {
            _name = name;
            _value = value;
            _actionParameterName = actionParameterName;
        }
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var formValue = filterContext.HttpContext.Request.Form[_name];
            filterContext.ActionArguments[_actionParameterName] = !string.IsNullOrEmpty(formValue) &&
                                                                   formValue.ToString().ToLowerInvariant().Equals(_value.ToLowerInvariant());
        }
    }
}