using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.GuardToolkit;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Tasks
{
    /// <summary>
    /// Defines the required contract for implementing a task executor.
    /// </summary>
    public interface ITaskEngine : ISingletonDependency
    {
        /// <summary>
        /// Executes any startup tasks.
        /// </summary>
        /// <param name="cancellationToken">[Optional] The cancellation token.</param>
        /// <returns>The task instance.</returns>
        Task RunShutdownTasksAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes any shutdown tasks.
        /// </summary>
        /// <param name="cancellationToken">[Optional] The cancellation token.</param>
        /// <returns>The task instance.</returns>
        Task RunStartupTasksAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Executes lifetime tasks.
    /// </summary>
    internal class TaskEngine : ITaskEngine
    {
        private readonly IServiceProvider _provider;

        public TaskEngine(IServiceProvider provider)
        {
            _provider = Ensure.IsNotNull(provider, nameof(provider));
        }

        /// <inheritdoc />
        public async Task RunShutdownTasksAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _provider.CreateScope())
            {
                var tasks = scope.ServiceProvider.GetServices<IShutdownTask>();

                foreach (var task in tasks)
                {
                    await task.RunAsync(cancellationToken);
                }
            }
        }

        /// <inheritdoc />
        public async Task RunStartupTasksAsync(CancellationToken cancellationToken = default)
        {
            using (var scope = _provider.CreateScope())
            {
                var tasks = scope.ServiceProvider.GetServices<IStartupTask>();

                foreach (var task in tasks)
                {
                    await task.RunAsync(cancellationToken);
                }
            }
        }
    }
}