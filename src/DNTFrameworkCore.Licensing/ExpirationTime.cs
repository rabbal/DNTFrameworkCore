using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

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

        public static ExpirationTime New(DateTime dateTime)
        {
            return new ExpirationTime(dateTime);
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

        public static explicit operator ExpirationTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? New(dateTime.Value) : Infinite;
        }

        public static implicit operator DateTime?(ExpirationTime dateTime)
        {
            return dateTime.Value;
        }

        public override string ToString()
        {
            return this == Infinite ? nameof(Infinite) : Value.ToString();
        }
    }
}