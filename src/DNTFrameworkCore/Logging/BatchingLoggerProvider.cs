using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Logging
{
    public abstract class BatchingLoggerProvider : ILoggerProvider
    {
        private readonly IList<LogItem> _batch = new List<LogItem>();
        private readonly TimeSpan _interval;
        private readonly int? _queueSize;
        private readonly int? _batchSize;
        private BlockingCollection<LogItem> _queue;
        private Task _outputTask;
        private CancellationTokenSource _cancellationTokenSource;
        private readonly IServiceProvider _provider;
        private int _messagesDropped;

        protected BatchingLoggerProvider(IOptions<BatchingLoggerOptions> options, IServiceProvider provider)
        {
            var loggerOptions = options.Value;
            if (loggerOptions.BatchSize <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.BatchSize),
                    $"{nameof(loggerOptions.BatchSize)} must be a positive number.");
            }

            if (loggerOptions.FlushPeriod <= TimeSpan.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(loggerOptions.FlushPeriod),
                    $"{nameof(loggerOptions.FlushPeriod)} must be longer than zero.");
            }

            _interval = loggerOptions.FlushPeriod;
            _batchSize = loggerOptions.BatchSize;
            _queueSize = loggerOptions.BackgroundQueueSize;
            _provider = provider;

            Start();
        }

        protected abstract Task WriteLogsAsync(IEnumerable<LogItem> items,
            CancellationToken cancellationToken);

        private async Task ProcessLogQueue(object state)
        {
            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                var limit = _batchSize ?? int.MaxValue;

                while (limit > 0 && _queue.TryTake(out var message))
                {
                    _batch.Add(message);
                    limit--;
                }

                var messagesDropped = Interlocked.Exchange(ref _messagesDropped, 0);
                if (messagesDropped != 0)
                {
                    _batch.Add(new LogItem
                    {
                        Message =
                            $"{messagesDropped} message(s) dropped because of queue size limit. Increase the queue size or decrease logging verbosity to avoid this.{Environment.NewLine}",
                        Level = LogLevel.Critical,
                        LoggerName = "ProcessLogQueue",
                        CreationTime = DateTime.UtcNow
                    });
                }

                if (_batch.Count > 0)
                {
                    try
                    {
                        await WriteLogsAsync(_batch, _cancellationTokenSource.Token);
                    }
                    catch
                    {
                        // ignored
                    }

                    _batch.Clear();
                }

                await IntervalAsync(_interval, _cancellationTokenSource.Token);
            }
        }

        protected virtual Task IntervalAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            return Task.Delay(interval, cancellationToken);
        }

        internal void Queue(LogItem item)
        {
            if (_queue.IsAddingCompleted) return;
            
            try
            {
                if (!_queue.TryAdd(item, 0, _cancellationTokenSource.Token))
                {
                    Interlocked.Increment(ref _messagesDropped);
                }
            }
            catch
            {
                //cancellation token canceled or CompleteAdding called
            }
        }

        private void Start()
        {
            _queue = !_queueSize.HasValue
                ? new BlockingCollection<LogItem>(new ConcurrentQueue<LogItem>())
                : new BlockingCollection<LogItem>(new ConcurrentQueue<LogItem>(), _queueSize.Value);

            _cancellationTokenSource = new CancellationTokenSource();
            _outputTask = Task.Factory.StartNew(
                ProcessLogQueue,
                null,
                TaskCreationOptions.LongRunning);
        }

        private void Stop()
        {
            _cancellationTokenSource.Cancel();
            _queue.CompleteAdding();

            try
            {
                _outputTask.Wait(_interval);
            }
            catch (TaskCanceledException)
            {
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count == 1 &&
                                                ex.InnerExceptions[0] is TaskCanceledException)
            {
            }
        }

        public void Dispose()
        {
            Stop();
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new BatchingLogger(this, _provider, categoryName);
        }
    }
}