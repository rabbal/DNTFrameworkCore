using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class Address : ValueObject
    {
        public string Street { get; private set; }
        public int Number { get; private set; }

        private Address()
        {
        }

        private Address(string street, int number)
        {
            Street = street;
            Number = number;
        }

        public static Result<Address> Create(string street, int number)
        {
            street = street ?? string.Empty;

            if (street.Length == 0)
            {
                return Result.Fail<Address>("street should not be empty");
            }

            if (street.Length > 100)
            {
                return Result.Fail<Address>("street is too long");
            }

            if (number <= 0)
            {
                return Result.Fail<Address>("number should not be negative or zero");
            }

            var address = new Address(street, number);

            return Result.Ok(address);
        }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                // Using a yield return statement to return each element one at a time
                yield return Street;
                yield return Number;
            }
        }
    }
}