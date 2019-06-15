using System.Collections.Generic;
using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class Product : Entity, IAggregateRoot, INumberedEntity
    {
        public string Title { get; set; }
        public byte[] RowVersion { get; set; }
        public string Number { get; set; }

        public ICollection<ProductPrice> Prices { get; set; }
    }
}