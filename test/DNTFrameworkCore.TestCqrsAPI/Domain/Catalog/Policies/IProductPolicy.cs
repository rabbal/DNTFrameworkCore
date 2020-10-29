using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies
{
    public interface IProductPolicy
    {
        bool IsUnique(Product product);
    }
}