using System;

namespace DNTFrameworkCore.Application.Models
{
    public class Id : Id<int>
    {
    }

    public class Id<TValue> where TValue : IEquatable<TValue>
    {
        public TValue Value { get; set; }
    }
}