using System;

namespace DNTFrameworkCore.Timing
{
    public static class SystemTime
    {
        public static Func<DateTimeOffset> NowFactory { get; set; } = () => DateTimeOffset.Now;
        public static Func<DateTimeOffset> UtcNowFactory { get; set; } = () => DateTimeOffset.UtcNow;

        public static DateTimeOffset Now => NowFactory();
        public static DateTimeOffset UtcNow => UtcNowFactory();
    }
}