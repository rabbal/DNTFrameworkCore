using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using DNTFrameworkCore.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Mvc
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2573
    /// </summary>
    public interface IMvcService
    {
        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application.
        /// </summary>
        ICollection<MvcControllerMetadata> MvcControllers { get; }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.
        /// </summary>
        ICollection<MvcControllerMetadata> FindSecuredControllerActionsWithPolicy(string policyName);
    }

    /// <summary>
    /// MvcActions Discovery Service Extensions
    /// </summary>
    public static class MvcServiceExtensions
    {
        /// <summary>
        /// Adds IMvcActionsDiscoveryService to IServiceCollection.
        /// </summary>
        public static IServiceCollection AddMvcActionsDiscoveryService(this IServiceCollection services)
        {
            services.AddSingleton<IMvcService, MvcService>();
            return services;
        }
    }

    /// <summary>
    /// MvcActions Discovery Service
    /// </summary>
    public class MvcService : IMvcService
    {
        private readonly IActionDescriptorCollectionProvider _actionProvider;

        private readonly LazyConcurrentDictionary<string, ICollection<MvcControllerMetadata>>
            _actionsWithPolicy = new LazyConcurrentDictionary<string, ICollection<MvcControllerMetadata>>();

        public MvcService(IActionDescriptorCollectionProvider actionProvider)
        {
            _actionProvider = actionProvider ??
                              throw new ArgumentNullException(
                                  nameof(actionProvider));

            MvcControllers = new List<MvcControllerMetadata>();

            var lastControllerName = string.Empty;
            MvcControllerMetadata currentController = null;

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
                    currentController = new MvcControllerMetadata
                    {
                        AreaName = controllerTypeInfo.GetCustomAttribute<AreaAttribute>()?.RouteValue,
                        Attributes = GetAttributes(controllerTypeInfo),
                        DisplayName =
                            controllerTypeInfo.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName,
                        Name = descriptor.ControllerName,
                    };
                    MvcControllers.Add(currentController);

                    lastControllerName = descriptor.ControllerName;
                }

                currentController?.Actions.Add(new MvcActionMetadata
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
        public ICollection<MvcControllerMetadata> MvcControllers { get; }

        /// <summary>
        /// Returns the list of all of the controllers and action methods of an MVC application which have AuthorizeAttribute and the specified policyName.
        /// </summary>
        public ICollection<MvcControllerMetadata> FindSecuredControllerActionsWithPolicy(string policyName)
        {
            var result = _actionsWithPolicy.GetOrAdd(policyName, y =>
            {
                var controllers = new List<MvcControllerMetadata>(MvcControllers);
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
                    var attributeNamespace = attribute.GetType().Namespace;
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

    public class MvcActionMetadata
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
                string.Join(",", Attributes.Select(a => a.GetType().Name.Replace(attribute, "")));
            return $"[{attributes}]{Name}";
        }
    }

    public class MvcControllerMetadata
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
        public IList<MvcActionMetadata> Actions { get; set; } = new List<MvcActionMetadata>();

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