using DNTFrameworkCore.Domain;
using DNTFrameworkCore.TestCqrsAPI.Domain.SharedKernel;

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