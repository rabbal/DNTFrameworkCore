using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Linq;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestWebApp.Application.Catalog.Models;
using DNTFrameworkCore.TestWebApp.Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Catalog
{
    public interface IProductService : ICrudService<long, ProductModel>
    {
    }

    public class ProductService : CrudService<Product, long, ProductModel>, IProductService
    {
        private readonly IMapper _mapper;

        public ProductService(
            IUnitOfWork uow,
            IEventBus bus,
            IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public override Task<IPagedResult<ProductModel>> ReadPagedListAsync(FilteredPagedRequestModel model,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking().Select(p => new ProductModel
            {
                Id = p.Id,
                Version = p.Version,
                Title = p.Title,
                Number = p.Number
            }).ToPagedListAsync(model, cancellationToken);
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