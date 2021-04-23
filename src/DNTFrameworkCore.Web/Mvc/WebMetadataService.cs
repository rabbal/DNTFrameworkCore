using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DNTFrameworkCore.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace DNTFrameworkCore.Web.Mvc
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2573
    /// </summary>
    public interface IWebMetadataService
    {
        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application.
        /// </summary>
        ICollection<ControllerMetadata> Controllers { get; }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.
        /// </summary>
        ICollection<ControllerMetadata> FindSecuredControllersWithPolicy(string policyName);
    }

    /// <summary>
    /// MvcActions Discovery Service
    /// </summary>
    public class WebMetadataService : IWebMetadataService
    {
        private readonly IActionDescriptorCollectionProvider _actionProvider;

        private readonly LockingConcurrentDictionary<string, ICollection<ControllerMetadata>>
            _actionsWithPolicy = new LockingConcurrentDictionary<string, ICollection<ControllerMetadata>>();

        public WebMetadataService(IActionDescriptorCollectionProvider actionProvider)
        {
            _actionProvider = actionProvider ??
                              throw new ArgumentNullException(
                                  nameof(actionProvider));

            Controllers = new List<ControllerMetadata>();

            var lastControllerName = string.Empty;
            ControllerMetadata currentController = null;

            var actionDescriptors = actionProvider.ActionDescriptors.Items;
            foreach (var actionDescriptor in actionDescriptors)
            {
                if (!(actionDescriptor is ControllerActionDescriptor descriptor))
                {
                    continue;
                }

                var controllerTypeInfo = descriptor.ControllerTypeInfo;
                var actionMethodInfo = descriptor.MethodInfo;

                if (lastControllerName != descriptor.ControllerName)
                {
                    currentController = new ControllerMetadata
                    {
                        AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                        Attributes = GetAttributes(controllerTypeInfo),
                        DisplayName =
                            controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                        Name = descriptor.ControllerName,
                    };
                    Controllers.Add(currentController);

                    lastControllerName = descriptor.ControllerName;
                }

                currentController?.Actions.Add(new ActionMetadata
                {
                    ControllerId = currentController.Id,
                    Name = descriptor.ActionName,
                    DisplayName = actionMethodInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                    Attributes = GetAttributes(actionMethodInfo),
                    Secured = IsSecuredAction(controllerTypeInfo, actionMethodInfo)
                });
            }
        }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application.
        /// </summary>
        public ICollection<ControllerMetadata> Controllers { get; }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.
        /// </summary>
        public ICollection<ControllerMetadata> FindSecuredControllersWithPolicy(string policyName)
        {
            var result = _actionsWithPolicy.GetOrAdd(policyName, y =>
            {
                var controllers = new List<ControllerMetadata>(Controllers);
                foreach (var controller in controllers)
                {
                    controller.Actions = controller.Actions.Where(
                        model => model.Secured &&
                                 (
                                     model.Attributes.OfType<AuthorizeAttribute>().FirstOrDefault()
                                         ?.Policy == policyName ||
                                     controller.Attributes.OfType<AuthorizeAttribute>()
                                         .FirstOrDefault()?.Policy == policyName
                                 )).ToList();
                }

                return controllers.Where(model => model.Actions.Any()).ToList();
            });

            return result;
        }

        private static List<Attribute> GetAttributes(MemberInfo actionMethodInfo)
        {
            return actionMethodInfo.GetCustomAttributes(inherit: true)
                .Where(attribute =>
                {
                    var attributeNamespace = attribute.GetType().GetTypeInfo().Namespace;
                    return attributeNamespace != typeof(CompilerGeneratedAttribute).Namespace &&
                           attributeNamespace != typeof(DebuggerStepThroughAttribute).Namespace;
                })
                .Cast<Attribute>()
                .ToList();
        }

        private static bool IsSecuredAction(MemberInfo controllerTypeInfo, MemberInfo actionMethodInfo)
        {
            var actionHasAllowAnonymousAttribute =
                actionMethodInfo.GetCustomAttribute<AllowAnonymousAttribute>(inherit: true) != null;
            if (actionHasAllowAnonymousAttribute)
            {
                return false;
            }

            var controllerHasAuthorizeAttribute =
                controllerTypeInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
            if (controllerHasAuthorizeAttribute)
            {
                return true;
            }

            var actionMethodHasAuthorizeAttribute =
                actionMethodInfo.GetCustomAttribute<AuthorizeAttribute>(inherit: true) != null;
            if (actionMethodHasAuthorizeAttribute)
            {
                return true;
            }

            return false;
        }
    }

    public class ActionMetadata
    {
        /// <summary>
        /// It's set to `{ControllerId}:{ActionName}`
        /// </summary>
        public string Id => $"{ControllerId}:{Name}";

        /// <summary>
        /// Returns `DisplayNameAttribute` value of the action method.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Return ControllerActionDescriptor.ActionName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Returns the list of Attributes of the action method.
        /// </summary>
        public IList<Attribute> Attributes { get; set; }

        /// <summary>
        /// It's set to `{AreaName}:{ControllerName}`
        /// </summary>
        public string ControllerId { get; set; }

        /// <summary>
        /// Returns true if the action method has an `AuthorizeAttribute`.
        /// </summary>
        public bool Secured { get; set; }

        /// <summary>
        /// Returns `[{Attributes}]{ActionName}`
        /// </summary>
        public override string ToString()
        {
            const string attribute = "Attribute";
            var attributes =
                string.Join(",", Attributes.Select(a => a.GetType().GetTypeInfo().Name.Replace(attribute, "")));
            return $"[{attributes}]{Name}";
        }
    }

    public class ControllerMetadata
    {
        /// <summary>
        /// It's set to `{AreaName}:{ControllerName}`
        /// </summary>
        public string Id => $"{AreaName}:{Name}";

        /// <summary>
        /// Returns the `DisplayNameAttribute` value
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Return ControllerActionDescriptor.ControllerName
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Return `AreaAttribute.RouteValue`
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// Returns the list of the Controller's Attributes.
        /// </summary>
        public IList<Attribute> Attributes { get; set; }

        /// <summary>
        /// Returns the list of the Controller's action methods.
        /// </summary>
        public IList<ActionMetadata> Actions { get; set; } = new List<ActionMetadata>();

        /// <summary>
        /// Returns `[{Attributes}]{AreaName}.{ControllerName}`
        /// </summary>
        public override string ToString()
        {
            const string attribute = "Attribute";
            var attributes = string.Join(",",
                Attributes.Select(a => a.GetType().Name.Replace(attribute, "")));
            return $"[{attributes}]{AreaName}.{Name}";
        }
    }
}