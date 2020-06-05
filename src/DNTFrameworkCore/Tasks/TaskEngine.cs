using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Tasks
{
    public interface ITaskEngine : IScopedDependency
    {
        Task RunOnStartup(CancellationToken cancellationToken = default);
        Task RunOnEnd(CancellationToken cancellationToken = default);
        Task RunOnException(Exception exception, CancellationToken cancellationToken = default);
        Task RunOnBeginRequest(CancellationToken cancellationToken = default);
        Task RunOnEndRequest(CancellationToken cancellationToken = default);
    }

    internal class TaskEngine : ITaskEngine
    {
        private readonly IEnumerable<ITask> _tasks;

        public TaskEngine(IEnumerable<ITask> tasks)
        {
            _tasks = tasks.OrderBy(task => task.Order).ToList();
        }

        public async Task RunOnStartup(CancellationToken cancellationToken = default)
        {
            foreach (var task in _tasks.OfType<IStartupTask>())
            {
                await task.Run(cancellationToken);
            }
        }

        public async Task RunOnEnd(CancellationToken cancellationToken = default)
        {
            foreach (var task in _tasks.OfType<IEndTask>())
            {
                await task.Run(cancellationToken);
            }
        }

        public async Task RunOnException(Exception exception, CancellationToken cancellationToken = default)
        {
            foreach (var task in _tasks.OfType<IExceptionTask>())
            {
                await task.Run(exception, cancellationToken);
            }
        }

        public async Task RunOnBeginRequest(CancellationToken cancellationToken = default)
        {
            foreach (var task in _tasks.OfType<IBeginRequestTask>())
            {
                await task.Run(cancellationToken);
            }
        }

        public async Task RunOnEndRequest(CancellationToken cancellationToken = default)
        {
            foreach (var task in _tasks.OfType<IEndRequestTask>())
            {
                await task.Run(cancellationToken);
            }
        }
    }
}