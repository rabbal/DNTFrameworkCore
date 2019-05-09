using System;

namespace DNTFrameworkCore.Timing
{
    public interface IDateTime
    {
        DateTimeOffset UtcNow { get; }
        DateTimeOffset Now { get; }
    }
    public class DateTime : IDateTime
    {
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
        public DateTimeOffset Now => DateTimeOffset.Now;
    }
}
