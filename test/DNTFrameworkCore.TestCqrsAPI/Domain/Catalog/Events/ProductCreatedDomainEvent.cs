using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events
{
    public sealed class ProductCreatedDomainEvent : DomainEvent
    {
        public ProductCreatedDomainEvent(Product product)
        {
            Product = product;
        }

        public Product Product { get;  }
    }
}