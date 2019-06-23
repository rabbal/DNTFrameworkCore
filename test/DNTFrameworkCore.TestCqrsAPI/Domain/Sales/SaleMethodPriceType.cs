using DNTFrameworkCore.Domain;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class SaleMethodPriceType : Entity
    {
        public int PriceTypeId { get; set; }
        public PriceType PriceType { get; set; }
        public int SaleMethodId { get; set; }
        public SaleMethod SaleMethod { get; set; }
    }
}