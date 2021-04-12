using System.Linq;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Persistence;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestCqrsAPI.Infrastructure.Repositories.Catalog
{
    public class ProductRepository : RepositoryBase<Product, long>, IProductRepository
    {
        public ProductRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        protected override IQueryable<Product> FindEntityQueryable => EntitySet.Include(p => p.Prices);
    }
}