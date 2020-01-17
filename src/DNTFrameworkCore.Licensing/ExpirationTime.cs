using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.Licensing
{
    public class ExpirationTime : ValueObject
    {
        public static readonly ExpirationTime Infinite = new ExpirationTime(null);

        private ExpirationTime(DateTime? value)
        {
            Value = value;
        }

        public DateTime? Value { get; }

        public bool Expired => this != Infinite && Value < DateTime.UtcNow;

        public static ExpirationTime New(DateTime clock)
        {
            return new ExpirationTime(clock);
        }

        public static ExpirationTime Parse(string value)
        {
            if (value == nameof(Infinite)) return Infinite;

            return (ExpirationTime) DateTime.Parse(value);
        }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Value; }
        }

        public static explicit operator ExpirationTime(DateTime? clock)
        {
            return clock.HasValue ? New(clock.Value) : Infinite;
        }

        public static implicit operator DateTime?(ExpirationTime clock)
        {
            return clock.Value;
        }

        public override string ToString()
        {
            return this == Infinite ? nameof(Infinite) : Value.ToString();
        }
    }
}