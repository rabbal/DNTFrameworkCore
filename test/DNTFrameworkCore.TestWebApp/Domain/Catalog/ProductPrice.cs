using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestWebApp.Domain.Catalog
{
    public class ProductPrice : TrackableEntity
    {
        public decimal Price { get; set; }
        public bool IsDefault { get; set; }

        public PriceType PriceType { get; set; }
        public int PriceTypeId { get; set; }
        public Product Product { get; set; }
        public long ProductId { get; set; }
    }
}