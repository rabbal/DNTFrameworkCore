using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DNTFrameworkCore.Web.Filters
{
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ParameterBasedOnFormNameAndValueAttribute : ActionFilterAttribute
    {
        private readonly string _actionParameterName;
        private readonly string _name;
        private readonly string _value;

        public ParameterBasedOnFormNameAndValueAttribute(string name, string value, string actionParameterName)
        {
            _name = name;
            _value = value;
            _actionParameterName = actionParameterName;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var formValue = filterContext.HttpContext.Request.Form[_name];
            filterContext.ActionArguments[_actionParameterName] = !string.IsNullOrEmpty(formValue) && formValue.Equals(_value);
        }
    }
}