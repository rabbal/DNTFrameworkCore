using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Logging
{
    /// <summary>
    /// Represents a log in the logging database.
    /// </summary>
    public class Log : Entity<Guid>
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string LoggerName { get; set; }
        public string UserBrowserName { get; set; }
        public string UserIP { get; set; }
        public long? UserId { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public int EventId { get; set; }
    }
}