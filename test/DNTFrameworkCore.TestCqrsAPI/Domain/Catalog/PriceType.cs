using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class PriceType : Entity, IAggregateRoot
    {
        public string Title { get; set; }
        public byte[] RowVersion { get; set; }
    }
}