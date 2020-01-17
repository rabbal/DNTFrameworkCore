﻿using System;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.Timing
{
    /// <summary>
    /// Defines the required contract for implementing a clock.
    /// </summary>
    public interface IClock : ITransientDependency
    {
        DateTime Now { get; }
        DateTime Normalize(DateTime dateTime);
    }

    internal sealed class Clock : IClock
    {
        public DateTime Now => DateTime.UtcNow;

        public DateTime Normalize(DateTime dateTime)
        {
            return DateTime.SpecifyKind(dateTime, DateTimeKind.Utc);
        }
    }
}