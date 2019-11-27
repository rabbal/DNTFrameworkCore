using System;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Timing
{
    /// <summary>
    /// Defines the required contract for implementing a clock.
    /// </summary>
    public interface IDateTime : ISingletonDependency
    {
        /// <summary>
        /// Gets the current date/time in the UTC timezone.
        /// </summary>
        DateTime UtcNow { get; }

        /// <summary>
        /// Gets the current date/time in the Local timezone.
        /// </summary>
        DateTime Now { get; }
    }

    internal sealed class SystemDateTime : IDateTime
    {
        public DateTime UtcNow => SystemTime.UtcNow;
        public DateTime Now => SystemTime.Now;
    }
}