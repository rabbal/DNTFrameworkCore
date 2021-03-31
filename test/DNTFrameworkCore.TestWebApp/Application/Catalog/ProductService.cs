using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Catalog.Models;
using DNTFrameworkCore.TestWebApp.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Catalog
{
    public interface IProductService : IEntityService<long, ProductModel>
    {
    }

    public class ProductService : EntityService<Product, long, ProductModel>, IProductService
    {
        private readonly IMapper _mapper;

        public ProductService(
            IDbContext dbContext,
            IEventBus bus,
            IMapper mapper) : base(dbContext, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override Task<IPagedResult<ProductModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            request.SortingIfEmpty("Id DESC");
            return EntitySet.AsNoTracking().Select(p => new ProductModel
            {
                Id = p.Id,
                Version = EFCoreShadow.PropertyVersion(p),
                Title = p.Title,
                Number = p.Number
            }).ToPagedListAsync(request, cancellationToken);
        }

        protected override void MapToEntity(ProductModel model, Product product)
        {
            _mapper.Map(model, product);
        }

        protected override ProductModel MapToModel(Product product)
        {
            return _mapper.Map<ProductModel>(product);
        }
    }
}