using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class Price : ValueObject
    {
        public long Value { get; private set; }
        public string Currency { get; private set; }

        private Price(long value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public static Result<Price> Create(long value, string currency)
        {
            if (value <= 0)
            {
                return Fail<Price>("value should not be negative or zero.");
            }

            var price = new Price(value, currency);

            return Ok(price);
        }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return Value;
                yield return Currency;
            }
        }
    }
}