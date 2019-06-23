using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class Product : TrackableEntity, IHasRowVersion, INumberedEntity
    {
        public ProductTitle Title { get; private set; }
        public byte[] RowVersion { get; private set; }
        public string Number { get; set; }

        public ICollection<ProductPrice> Prices { get; set; }
    }
}