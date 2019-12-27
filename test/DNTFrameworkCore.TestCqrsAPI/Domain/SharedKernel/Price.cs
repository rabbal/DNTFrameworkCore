using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel
{
    public class Price : ValueObject
    {
        private Price()
        {
        }

        private Price(decimal value, string currency)
        {
            Value = value;
            Currency = currency;
        }

        public decimal Value { get; private set; }
        public string Currency { get; private set; }

        public static Result<Price> New(decimal value, string currency)
        {
            if (value < 0) return Fail<Price>("value should not be negative");

            return string.IsNullOrWhiteSpace(currency)
                ? Fail<Price>("currency should not be empty")
                : Ok(new Price(value, currency));
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