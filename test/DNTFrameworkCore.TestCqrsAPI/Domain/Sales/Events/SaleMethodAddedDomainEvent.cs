using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Events
{
    public class SaleMethodAddedDomainEvent : DomainEvent
    {
        public long SaleMethodId { get; }

        public SaleMethodAddedDomainEvent(long saleMethodId)
        {
            SaleMethodId = saleMethodId;
        }
    }
}