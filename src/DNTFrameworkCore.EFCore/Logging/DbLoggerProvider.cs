using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
// ReSharper disable InconsistentNaming

namespace DNTFrameworkCore.EFCore.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ILoggingBuilder AddEFCore<TContext>(this ILoggingBuilder builder)
         where TContext : DbContext
        {
            builder.Services.AddSingleton<ILoggerProvider, DbLoggerProvider<TContext>>();

            return builder;
        }
        public static ILoggingBuilder AddEFCore<TContext>(this ILoggingBuilder builder, Action<DbLoggerOptions>
            configuration)
            where TContext : DbContext
        {
            builder.AddEFCore<TContext>();
            builder.Services.Configure(configuration);

            return builder;
        }
    }

    [ProviderAlias("EFCore")]
    internal class DbLoggerProvider<TContext> : BatchingLoggerProvider
        where TContext : DbContext
    {
        private readonly IServiceProvider _provider;

        public DbLoggerProvider(IOptions<DbLoggerOptions> options, IServiceProvider provider) : base(options, provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
        protected override async Task WriteMessagesAsync(IEnumerable<LogMessage> messages, CancellationToken token)
        {
            var logs = messages.Where(m => !string.IsNullOrEmpty(m.Message))
                .Select(m => new Log
                {
                    Level = m.Level.ToString(),
                    LoggerName = m.LoggerName,
                    EventId = m.EventId.Id,
                    Message = m.Message,
                    UserIP = m.UserIP,
                    UserId = m.UserId,
                    UserBrowserName = m.UserBrowserName,
                    UserDisplayName = m.UserDisplayName,
                    UserName = m.UserName,
                    Timestamp = m.Timestamp
                });

            using (var scope = _provider.CreateScope())
            {
                using (var context = scope.ServiceProvider.GetRequiredService<TContext>())
                {
                    context.Set<Log>().AddRange(logs);
                    await context.SaveChangesAsync(token);
                }
            }
        }
    }
}