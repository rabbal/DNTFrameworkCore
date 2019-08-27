using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Parties
{
    public class Customer : AggregateRoot, IHasRowVersion, INumberedEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] RowVersion { get; set; }
        public string Number { get; private set; }
    }
}