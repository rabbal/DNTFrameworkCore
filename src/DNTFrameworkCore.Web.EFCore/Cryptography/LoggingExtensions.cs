using System;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.EFCore.Cryptography
{
    internal static class LoggingExtensions
    {
        private static readonly Action<ILogger, string, Exception> ExceptionOccurredWhileParsingKeyXml;
        private static readonly Action<ILogger, string, string, Exception> SavingKeyToDbContext;

        static LoggingExtensions()
        {
            ExceptionOccurredWhileParsingKeyXml = LoggerMessage.Define<string>(
                eventId: new EventId(1, "ExceptionOccurredWhileParsingKeyXml"),
                logLevel: LogLevel.Warning,
                formatString: "An exception occurred while parsing the key xml '{XmlValue}'.");
            SavingKeyToDbContext = LoggerMessage.Define<string, string>(
                eventId: new EventId(2, "SavingKeyToDbContext"),
                logLevel: LogLevel.Debug,
                formatString: "Saving key '{FriendlyName}' to '{DbContext}'.");
        }

        public static void LogExceptionWhileParsingKeyXml(this ILogger logger, string keyXml, Exception exception)
            => ExceptionOccurredWhileParsingKeyXml(logger, keyXml, exception);

        public static void LogSavingKeyToDbContext(this ILogger logger, string friendlyName, string contextName)
            => SavingKeyToDbContext(logger, friendlyName, contextName, null);
    }
}