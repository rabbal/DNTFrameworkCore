using System;
using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.EntityFramework.Logging
{
    /// <summary>
    /// Represents a log in the logging database.
    /// </summary>
    public class Log : CreationTrackingEntity<Guid>
    {
        public string Message { get; set; }
        public string Level { get; set; }
        public string Logger { get; set; }
        public int EventId { get; set; }
    }
}