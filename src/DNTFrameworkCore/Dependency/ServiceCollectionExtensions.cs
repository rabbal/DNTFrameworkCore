using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Dependency
{
    public static class ServiceCollectionExtensions
    {
        public static void RemoveService(this IServiceCollection services, Type serviceType)
        {
            var serviceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == serviceType);
            if (serviceDescriptor != null)
            {
                services.Remove(serviceDescriptor);
            }
        }

        public static IServiceCollection CloneSingleton(
            this IServiceCollection services,
            ServiceDescriptor parent,
            object implementationInstance)
        {
            var cloned = new ClonedSingletonDescriptor(parent, implementationInstance);
            services.Add(cloned);
            return services;
        }

        public static IServiceCollection CloneSingleton(
            this IServiceCollection collection,
            ServiceDescriptor parent,
            Func<IServiceProvider, object> implementationFactory)
        {
            var cloned = new ClonedSingletonDescriptor(parent, implementationFactory);
            collection.Add(cloned);
            return collection;
        }

        /// <summary>
        /// Executes the specified action if the specified <paramref name="condition"/> is <c>true</c> which can be
        /// used to conditionally configure the MVC services.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="condition">If set to <c>true</c> the action is executed.</param>
        /// <param name="action">The action used to configure the MVC services.</param>
        /// <returns>The same services collection.</returns>
        public static IServiceCollection AddIf(
            this IServiceCollection services,
            bool condition,
            Func<IServiceCollection, IServiceCollection> action)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (condition)
            {
                services = action(services);
            }

            return services;
        }

        /// <summary>
        /// Executes the specified <paramref name="ifAction"/> if the specified <paramref name="condition"/> is
        /// <c>true</c>, otherwise executes the <paramref name="elseAction"/>. This can be used to conditionally
        /// configure the MVC services.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="condition">If set to <c>true</c> the <paramref name="ifAction"/> is executed, otherwise the
        /// <paramref name="elseAction"/> is executed.</param>
        /// <param name="ifAction">The action used to configure the MVC services if the condition is <c>true</c>.</param>
        /// <param name="elseAction">The action used to configure the MVC services if the condition is <c>false</c>.</param>
        /// <returns>The same services collection.</returns>
        public static IServiceCollection AddIfElse(
            this IServiceCollection services,
            bool condition,
            Func<IServiceCollection, IServiceCollection> ifAction,
            Func<IServiceCollection, IServiceCollection> elseAction)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (ifAction == null)
            {
                throw new ArgumentNullException(nameof(ifAction));
            }

            if (elseAction == null)
            {
                throw new ArgumentNullException(nameof(elseAction));
            }

            services = condition ? ifAction(services) : elseAction(services);

            return services;
        }
    }
}