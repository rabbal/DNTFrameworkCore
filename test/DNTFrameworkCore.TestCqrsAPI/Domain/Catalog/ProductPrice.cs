using DNTFrameworkCore.Domain;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog
{
    public class ProductPrice : Entity
    {
        private ProductPrice() //ORM
        {
        }

        internal ProductPrice(Product product, PriceType priceType, Price price)
        {
            Product = product;
            PriceType = priceType;
            Price = price;
        }

        public Price Price { get; private set; }
        public PriceType PriceType { get; private set; }
        public Product Product { get; private set; }
        public bool IsDefault { get; private set; }

        internal void MarkIsDefault()
        {
            IsDefault = true;
        }
        
        internal void UnmarkIsDefault()
        {
            IsDefault = false;
        }
    }
}