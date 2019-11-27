using System;

namespace DNTFrameworkCore.Timing
{
    public static class SystemTime
    {
        public static Func<DateTime> NowFactory { get; set; } = () => DateTime.Now;
        public static Func<DateTime> UtcNowFactory { get; set; } = () => DateTime.UtcNow;

        /// <summary>
        /// Gets the current date/time in the Local timezone.
        /// </summary>
        public static DateTime Now => NowFactory();
        /// <summary>
        /// Gets the current date/time in the UTC timezone.
        /// </summary>
        public static DateTime UtcNow => UtcNowFactory();
    }
}