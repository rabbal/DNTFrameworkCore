using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Threading.BackgroundTasks
{
    public interface IBackgroundTaskQueue : ISingletonDependency
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, IServiceProvider, Task> workItem);

        Task<Func<CancellationToken, IServiceProvider, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }

    internal class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, IServiceProvider, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, IServiceProvider, Task>>();

        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(
            Func<CancellationToken, IServiceProvider, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, IServiceProvider, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}