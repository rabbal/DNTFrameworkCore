using System;
using System.Collections.Generic;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Catalog
{
    public class Product : Entity<long>, IRowVersion, ICreationTracking, IModificationTracking,
        INumberedEntity
    {
        public const int MaxTitleLength = 50;

        public string Title { get; set; }
        public string Number { get; set; }
        public byte[] Version { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public DateTime? ModifiedDateTime { get; set; }

        public ICollection<ProductPrice> Prices { get; set; }
    }
}