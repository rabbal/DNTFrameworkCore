using System;

namespace DNTFrameworkCore.Timing
{
    public interface IClock
    {
        DateTimeOffset UtcNow { get; }
        DateTimeOffset Now { get; }
    }
    public class Clock : IClock
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
