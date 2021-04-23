using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Persistence;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories;

namespace DNTFrameworkCore.TestCqrsAPI.Infrastructure.Repositories.Catalog
{
    public class PriceTypeRepository : RepositoryBase<PriceType, long>, IPriceTypeRepository
    {
        public PriceTypeRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}