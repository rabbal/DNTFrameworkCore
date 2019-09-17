using System;

namespace DNTFrameworkCore.Timing
{
    public static class SystemTime
    {
        public static Func<DateTimeOffset> NowFactory { get; set; } = () => DateTimeOffset.Now;
        public static Func<DateTimeOffset> UtcNowFactory { get; set; } = () => DateTimeOffset.UtcNow;

        /// <summary>
        /// Gets the current date/time in the Local timezone.
        /// </summary>
        public static DateTimeOffset Now => NowFactory();
        /// <summary>
        /// Gets the current date/time in the UTC timezone.
        /// </summary>
        public static DateTimeOffset UtcNow => UtcNowFactory();
    }
}