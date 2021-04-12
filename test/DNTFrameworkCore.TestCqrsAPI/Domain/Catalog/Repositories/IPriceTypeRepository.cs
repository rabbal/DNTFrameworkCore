using DNTFrameworkCore.Domain;

namespace DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories
{
    public interface IPriceTypeRepository : IRepository<PriceType, long>
    {
    }
}