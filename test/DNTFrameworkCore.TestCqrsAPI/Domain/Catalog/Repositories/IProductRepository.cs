using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories
{
    public interface IProductRepository : IRepository<Product, long>
    {
    }
}