using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Orders
{
    public class CustomerId : ValueObject
    {
        public int Id { get; private set; }

        private CustomerId()
        {
        }

        protected override IEnumerable<object> EqualityValues
        {
            get { yield return Id; }
        }

        public Result<CustomerId> Create(int id)
        {
            //todo: check customer should be exist or other rules...
            var customerId = new CustomerId {Id = id};
            return Ok(customerId);
        }
    }
}