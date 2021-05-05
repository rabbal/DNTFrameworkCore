using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events
{
    public sealed class ProductAddedDomainEvent : DomainEvent
    {
        public ProductAddedDomainEvent(Product product)
        {
            Product = product;
        }

        public Product Product { get;  }
    }
}