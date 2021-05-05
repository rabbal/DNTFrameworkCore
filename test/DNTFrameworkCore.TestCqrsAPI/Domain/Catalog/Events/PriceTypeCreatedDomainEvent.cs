using System;
using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Events
{
    public sealed class PriceTypeCreatedDomainEvent : DomainEvent
    {
        public PriceTypeCreatedDomainEvent(PriceType priceType)
        {
            PriceType = priceType ?? throw new ArgumentNullException(nameof(priceType));
        }

        public PriceType PriceType { get; }
    }
}