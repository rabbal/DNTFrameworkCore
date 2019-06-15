using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Parties
{
    public class Customer : Entity, IAggregateRoot, INumberedEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public byte[] RowVersion { get; set; }
        public string Number { get; set; }
    }
}