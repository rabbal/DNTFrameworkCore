using System;
using System.Text;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Runtime;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Logging
{
        public class BatchingLogger : ILogger
    {
        private readonly BatchingLoggerProvider _loggerProvider;
        private readonly string _loggerName;
        private readonly IServiceProvider _provider;

        public BatchingLogger(BatchingLoggerProvider loggerProvider, IServiceProvider provider, string loggerName)
        {
            _loggerProvider = loggerProvider;
            _loggerName = loggerName;
            _provider = provider;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return NoopDisposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            if (logLevel == LogLevel.None)
            {
                return false;
            }

            return true;
        }

        public void Log<TState>(DateTimeOffset timestamp, LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var builder = new StringBuilder();
            builder.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss.fff zzz"));
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
            _provider.RunScoped<IUserSession>(session =>
            {
                _loggerProvider.AddMessage(new LogMessage
                {
                    Timestamp = timestamp,
                    Message = message,
                    LoggerName = _loggerName,
                    Level = logLevel,
                    EventId = eventId,
                    UserBrowserName = session.UserBrowserName,
                    UserIP = session.UserIP,
                    UserId = session.UserId,
                    UserName = session.UserName,
                    UserDisplayName = session.UserDisplayName
                });
            });
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Log(DateTimeOffset.UtcNow, logLevel, eventId, state, exception, formatter);
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