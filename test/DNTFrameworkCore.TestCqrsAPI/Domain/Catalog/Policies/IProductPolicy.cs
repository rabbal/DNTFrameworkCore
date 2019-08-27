using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies
{
    public interface IProductPolicy : IPolicy
    {
        bool IsUnique(Product product);
    }
}