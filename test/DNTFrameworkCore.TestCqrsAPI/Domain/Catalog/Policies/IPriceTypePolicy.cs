using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies
{
    public interface IPriceTypePolicy : IPolicy
    {
        bool IsUnique(PriceType priceType);
    }
}