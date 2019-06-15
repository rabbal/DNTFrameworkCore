using DNTFrameworkCore.Domain.Entities;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class ProductPrice : Entity
    {
        public decimal Price { get; set; }
        
        public int PriceTypeId { get; set; }
        public PriceType PriceType { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}