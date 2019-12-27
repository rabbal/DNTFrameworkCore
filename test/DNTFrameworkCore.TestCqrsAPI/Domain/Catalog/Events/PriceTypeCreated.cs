using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events
{
    public sealed class PriceTypeCreated : DomainEvent
    {
        public PriceTypeCreated(PriceType priceType)
        {
            PriceType = priceType ?? throw new ArgumentNullException(nameof(priceType));
        }

        public PriceType PriceType { get; }
    }
}