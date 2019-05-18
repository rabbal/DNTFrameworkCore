using System;
using System.Threading.Tasks;
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
                var service = scope.ServiceProvider.GetRequiredService<TS>();
                callback(service);
                if (service is IDisposable disposable)
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
                var service = scope.ServiceProvider.GetRequiredService<TS>();
                return callback(service);
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task RunScoped<T, TS>(this IServiceProvider provider, Func<TS, T, Task> callback)
        {
            using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<TS>();

                await callback(service, scope.ServiceProvider.GetRequiredService<T>());
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task RunScoped<TS>(this IServiceProvider provider, Func<TS, Task> callback)
        {
            using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<TS>();
                await callback(service);
                if (service is IDisposable disposable)
                {
                    disposable.Dispose();
                }
            }
        }

        /// <summary>
        /// Creates an IServiceScope which contains an IServiceProvider used to resolve dependencies from a newly created scope and then runs an associated callback.
        /// </summary>
        public static async Task<T> RunScoped<T, TS>(this IServiceProvider provider, Func<TS, Task<T>> callback)
        {
            using (var scope = provider.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var service = scope.ServiceProvider.GetRequiredService<TS>();
                return await callback(service);
            }
        }
    }
}