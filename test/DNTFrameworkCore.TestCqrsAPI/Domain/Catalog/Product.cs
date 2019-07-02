using System.Collections.Generic;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class Product : TrackableEntity, IHasRowVersion, INumberedEntity, ICreationTracking, IModificationTracking
    {
        public Title Title { get; private set; }
        public byte[] RowVersion { get; private set; }
        public string Number { get; set; }

        public ICollection<ProductPrice> Prices { get; set; }
    }
}