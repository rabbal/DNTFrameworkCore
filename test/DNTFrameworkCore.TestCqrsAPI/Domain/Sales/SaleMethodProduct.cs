using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class SaleMethodProduct : Entity
    {
        public Product Product { get; set; }
        public int ProductId { get; set; }
        public SaleMethod SaleMethod { get; set; }
        public int SaleMethodId { get; set; }
    }
}