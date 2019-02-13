using System;

namespace DNTFrameworkCore.Logging
{
    public struct LogMessage
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }
}