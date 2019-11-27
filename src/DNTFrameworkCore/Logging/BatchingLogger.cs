using System;
using System.Text;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Tenancy;
using DNTFrameworkCore.Timing;
using Microsoft.Extensions.DependencyInjection;
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

        public void Log<TState>(DateTime timestamp, LogLevel logLevel, EventId eventId, TState state,
            Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            var builder = new StringBuilder();
            builder.Append(timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
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
            using (var scope = _provider.CreateScope())
            {
                var user = scope.ServiceProvider.GetService<IUserSession>();
                var tenant = scope.ServiceProvider.GetService<ITenantSession>();

                _loggerProvider.AddMessage(new LogMessage
                {
                    CreationTime = timestamp,
                    Message = message,
                    LoggerName = _loggerName,
                    Level = logLevel,
                    EventId = eventId,
                    UserBrowserName = user?.UserBrowserName,
                    UserIP = user?.UserIP,
                    UserId = user?.UserId,
                    UserName = user?.UserName,
                    UserDisplayName = user?.UserDisplayName,
                    TenantId = tenant?.TenantId,
                    TenantName = tenant?.TenantName,
                    ImpersonatorUserId = user?.ImpersonatorUserId,
                    ImpersonatorTenantId = tenant?.ImpersonatorTenantId
                });
            }
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter)
        {
            Log(_provider.GetService<IDateTime>().UtcNow, logLevel, eventId, state, exception, formatter);
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