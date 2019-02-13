using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.EntityFramework.Logging
{
    public class DbLoggerOptions
    {
        public LogLevel MinLevel { get; set; } = LogLevel.Warning;
    }
}