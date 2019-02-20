using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Dependency
{
    public static class DependencyExtensions
    {
        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScoped<T, TS>(this IServiceProvider provider, Action<TS, T> callback)
        {
            using (var scope = provider.CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<TS>();

                callback(service, scope.ServiceProvider.GetRequiredService<T>());
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static void RunScoped<TS>(this IServiceProvider provider, Action<TS> callback)
        {
            using (var scope = provider.CreateScope())
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
        public static T RunScoped<T, TS>(this IServiceProvider provider, Func<TS, T> callback)
        {
            using (var scope = provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TS>();
                return callback(context);
            }
        }
    }
}