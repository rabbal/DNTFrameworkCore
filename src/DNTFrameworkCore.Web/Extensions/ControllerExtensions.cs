using System;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class ControllerExtensions
    {
        /// <summary>
        /// Gets the Controller's Name
        /// </summary>
        public static string ControllerName(this Type controllerType)
        {
            var baseType = typeof(Controller);
            if (!baseType.GetTypeInfo().IsAssignableFrom(controllerType))
            {
                throw new InvalidOperationException("This method should be used for `Microsoft.AspNetCore.Mvc.Controller`s.");
            }

            var lastControllerIndex = controllerType.Name.LastIndexOf("Controller", StringComparison.Ordinal);
            if (lastControllerIndex > 0)
            {
                return controllerType.Name.Substring(0, lastControllerIndex);
            }

            throw new InvalidOperationException("This type's name doesn't end with `Controller`.");
        }
    }
}