using System;
using System.Text;
using DNTFrameworkCore.Dependency;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.EntityFramework.Logging
{
    public class DbLogger<TContext> : ILogger
        where TContext : DbContext
    {
        private readonly string _loggerName;
        private readonly IServiceProvider _provider;

        public DbLogger(
            IServiceProvider provider,
            string loggerName)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _loggerName = string.IsNullOrEmpty(loggerName) ? "DbLogger" : loggerName;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel)) return;

            try
            {
                var builder = new StringBuilder();
                builder.Append(" [");
                builder.Append(logLevel.ToString());
                builder.Append("] ");
                builder.Append(_loggerName);
                builder.Append(" [");
                builder.Append(eventId.ToString());
                builder.Append("] ");
                builder.Append(": ");
                builder.AppendLine(formatter(state, exception));

                if (exception != null)
                {
                    builder.AppendLine(exception.ToString());
                }

                var message = builder.ToString();

                if (string.IsNullOrEmpty(message)) return;

                var log = new Log
                {
                    EventId = eventId.Id,
                    Level = logLevel.ToString(),
                    Logger = _loggerName,
                    Message = message
                };

                _provider.RunScoped<TContext>(context =>
                {
                    context.Set<Log>().Add(log);
                    context.SaveChanges();
                });
            }
            catch
            {
                // don't throw exceptions from logger
            }
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (_loggerName.StartsWith("Microsoft.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase))
                return false;

            return _provider.GetService<IOptions<DbLoggerOptions>>().Value.MinLevel > logLevel;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        private class NoopDisposable : IDisposable
        {
            static NoopDisposable()
            {
            }

            public static NoopDisposable Instance { get; } = new NoopDisposable();

            public void Dispose()
            {
            }
        }
    }
}