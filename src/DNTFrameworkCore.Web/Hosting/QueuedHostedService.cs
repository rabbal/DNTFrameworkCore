using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Threading.BackgroundTasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Hosting
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _factory;
        private readonly ILogger _logger;
        private readonly IBackgroundTaskQueue _queue;

        public QueuedHostedService(
            IBackgroundTaskQueue queue,
            IServiceScopeFactory factory,
            ILoggerFactory loggerFactory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            _logger = loggerFactory.CreateLogger<QueuedHostedService>();
        }


        protected override async Task ExecuteAsync(CancellationToken cancellationToken)

        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await _queue.DequeueAsync(cancellationToken);

                try
                {
                    using (var scope = _factory.CreateScope())
                    {
                        await workItem(cancellationToken, scope.ServiceProvider);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                        $"Error occurred executing {nameof(workItem)}.");
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }
    }
}