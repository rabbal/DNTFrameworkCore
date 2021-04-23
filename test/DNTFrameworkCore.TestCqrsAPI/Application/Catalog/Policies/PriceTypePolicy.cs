using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;

namespace DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Policies
{
    public class PriceTypePolicy : IPriceTypePolicy
    {
        public bool IsUnique(PriceType priceType)
        {
            throw new System.NotImplementedException();
        }
    }
}