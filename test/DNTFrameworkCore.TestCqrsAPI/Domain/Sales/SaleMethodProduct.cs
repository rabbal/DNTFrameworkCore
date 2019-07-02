using DNTFrameworkCore.Domain;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Sales
{
    public class SaleMethodProduct : Entity
    {
        public Product Product { get; private set; }
        public int ProductId { get;  private set; }
        private SaleMethodProduct()
        {
        }
        public SaleMethodProduct(int productId)
        {
            ProductId = productId;
        }
    }
}