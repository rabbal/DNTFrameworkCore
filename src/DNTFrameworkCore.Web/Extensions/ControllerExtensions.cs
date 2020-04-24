﻿using System;
using System.Reflection;
using System.Threading.Tasks;
using DNTFrameworkCore.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class ControllerExtensions
    {
        public static async Task<bool> HasPermission(this ControllerBase controllerBase, string permissionName)
        {
            var policyName = PermissionConstant.PolicyPrefix + permissionName;
            var authorization = controllerBase.HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            return (await authorization.AuthorizeAsync(controllerBase.User, policyName)).Succeeded;
        }

        /// <summary>
        /// Gets the Controller's Name
        /// </summary>
        public static string ControllerName(this Type controllerType)
        {
            var baseType = typeof(Controller);
            if (!baseType.GetTypeInfo().IsAssignableFrom(controllerType))
            {
                throw new InvalidOperationException(
                    "This method should be used for `Microsoft.AspNetCore.Mvc.Controller`s.");
            }

            var lastControllerIndex = controllerType.Name.LastIndexOf("Controller", StringComparison.Ordinal);
            if (lastControllerIndex > 0)
            {
                return controllerType.Name.Substring(0, lastControllerIndex);
            }

            throw new InvalidOperationException("This type's name doesn't end with `Controller`.");
        }

        public static void AddInfoToastNotification(this Controller controller, string message)
        {
            controller.TempData.AddInfoToastNotification(message);
        }

        public static void AddSuccessToastNotification(this Controller controller, string message)
        {
            controller.TempData.AddSuccessToastNotification(message);
        }

        public static void AddErrorToastNotification(this Controller controller, string message)
        {
            controller.TempData.AddErrorToastNotification(message);
        }

        public static void AddWarningToastNotification(this Controller controller, string message)
        {
            controller.TempData.AddWarningToastNotification(message);
        }

        public static IActionResult ErrorNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "error", __notification = true});
        }

        public static IActionResult SuccessNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "success", __notification = true});
        }

        public static IActionResult WarningNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "warning", __notification = true});
        }

        public static IActionResult InformationNotification(this Controller controller, string message)
        {
            return controller.Json(new {message = message, type = "info", __notification = true});
        }

        public static IActionResult ErrorMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "error", __message = true});
        }

        public static IActionResult SuccessMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "success", __message = true});
        }

        public static IActionResult WarningMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "warning", __message = true});
        }

        public static IActionResult InformationMessage(this Controller controller, string title, string message)
        {
            return controller.Json(new {title = title, message = message, type = "info", __message = true});
        }
    }
}