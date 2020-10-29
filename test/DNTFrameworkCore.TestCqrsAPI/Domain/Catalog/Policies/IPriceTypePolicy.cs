using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies
{
    public interface IPriceTypePolicy
    {
        bool IsUnique(PriceType priceType);
    }
}