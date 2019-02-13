using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Dependency
{
    public static class DependencyExtensions
    {
        public static void RegisterAllTypes<T>(this IServiceCollection services, Assembly[] assemblies,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            var typesFromAssemblies =
                assemblies.SelectMany(a => a.DefinedTypes.Where(x => x.GetInterfaces().Contains(typeof(T))));
            services.RegisterAllTypes<T>(typesFromAssemblies);
        }

        public static void RegisterAllTypes<T>(this IServiceCollection services, IEnumerable<Type> types,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            foreach (var type in types)
                services.Add(new ServiceDescriptor(typeof(T), type, lifetime));
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScoped<T, TS>(this IServiceProvider serviceProvider, Action<TS, T> callback)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TS>();

                callback(context, scope.ServiceProvider.GetRequiredService<T>());
                if (context is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScoped<TS>(this IServiceProvider serviceProvider, Action<TS> callback)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TS>();
                callback(context);
                if (context is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static T RunScoped<T, TS>(this IServiceProvider serviceProvider, Func<TS, T> callback)
        {
            using (var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TS>();
                return callback(context);
            }
        }
    }
}