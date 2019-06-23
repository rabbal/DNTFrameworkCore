using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class ProductPrice : TrackableEntity
    {
        public decimal Price { get; set; }
        
        public int PriceTypeId { get; set; }
        public PriceType PriceType { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}