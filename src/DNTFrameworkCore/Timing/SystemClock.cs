using System;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Timing
{
    public interface ISystemClock : ISingletonDependency
    {
        DateTime Now { get; }
        DateTime Normalize(DateTime dateTime);
    }

    internal sealed class SystemClock : ISystemClock
    {
        public DateTime Now => SystemTime.Now();

        public DateTime Normalize(DateTime dateTime)
        {
            return SystemTime.Normalize(dateTime);
        }
    }
}