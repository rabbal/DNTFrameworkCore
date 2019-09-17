using System;

namespace DNTFrameworkCore.Timing
{
    /// <summary>
    /// Defines the required contract for implementing a clock.
    /// </summary>
    public interface IDateTime
    {
        /// <summary>
        /// Gets the current date/time in the UTC timezone.
        /// </summary>
        DateTimeOffset UtcNow { get; }
        /// <summary>
        /// Gets the current date/time in the Local timezone.
        /// </summary>
        DateTimeOffset Now { get; }
    }

    internal class DateTime : IDateTime
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}