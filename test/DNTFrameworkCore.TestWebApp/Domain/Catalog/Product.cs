using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Domain.Entities.Tracking;

namespace DNTFrameworkCore.TestWebApp.Domain.Catalog
{
    public class Product : TrackableEntity<long>, IAggregateRoot, INumberedEntity
    {
        public const int MaxTitleLength = 50;
        
        public string Title { get; set; }
        public string Number { get; set; }
        public decimal Price { get; set; }
        public byte[] RowVersion { get; set; }
    }
}