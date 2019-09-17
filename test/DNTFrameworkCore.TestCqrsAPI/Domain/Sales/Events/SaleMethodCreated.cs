using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales.Events
{
    public class SaleMethodCreated : DomainEvent
    {
        public long SaleMethodId { get; }

        public SaleMethodCreated(long saleMethodId)
        {
            SaleMethodId = saleMethodId;
        }
    }
}