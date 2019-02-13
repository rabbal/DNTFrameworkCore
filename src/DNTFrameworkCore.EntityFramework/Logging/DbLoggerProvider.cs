using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.EntityFramework.Logging
{
    public static class LoggerFactoryExtensions
    {
        public static ILoggingBuilder AddEntityFramework<TContext>(this ILoggingBuilder builder, Action<DbLoggerOptions>
            configuration)
            where TContext : DbContext
        {
            builder.Services.Configure(configuration);
            builder.Services.AddSingleton<ILoggerProvider, DbLoggerProvider<TContext>>();

            return builder;
        }
    }

    [ProviderAlias("EntityFramework")]
    internal class DbLoggerProvider<TContext> : ILoggerProvider
        where TContext : DbContext
    {
        private readonly IServiceProvider _provider;

        public DbLoggerProvider(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public ILogger CreateLogger(string categoryName)
        {
            return new DbLogger<TContext>(_provider, categoryName);
        }

        public void Dispose()
        {
        }
    }
}