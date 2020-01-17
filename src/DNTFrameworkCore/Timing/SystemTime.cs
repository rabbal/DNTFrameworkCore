using System;

namespace DNTFrameworkCore.Timing
{
    public static class SystemTime
    {
        public static Func<DateTime> Now = () => DateTime.UtcNow;
        public static DateTime Normalize(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}