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
            _tasks = tasks.OrderBy(task => task.Order);
        }

        public Task RunOnStartup(CancellationToken cancellationToken = default)
        {
            var tasks = _tasks.OfType<IStartupTask>().Select(task => task.Run(cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task RunOnEnd(CancellationToken cancellationToken = default)
        {
            var tasks = _tasks.OfType<IEndTask>().Select(task => task.Run(cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task RunOnException(Exception exception, CancellationToken cancellationToken = default)
        {
            var tasks = _tasks.OfType<IExceptionTask>().Select(task => task.Run(exception, cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task RunOnBeginRequest(CancellationToken cancellationToken = default)
        {
            var tasks = _tasks.OfType<IBeginRequestTask>().Select(task => task.Run(cancellationToken));
            return Task.WhenAll(tasks);
        }

        public Task RunOnEndRequest(CancellationToken cancellationToken = default)
        {
            var tasks = _tasks.OfType<IEndRequestTask>().Select(task => task.Run(cancellationToken));
            return Task.WhenAll(tasks);
        }
    }
}