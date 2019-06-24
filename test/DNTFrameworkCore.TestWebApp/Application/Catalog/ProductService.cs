using System;
using System.Linq;
using AutoMapper;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Eventing;
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

        protected override IQueryable<ProductModel> BuildReadQuery(FilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking().Select(p => new ProductModel
            {
                Id = p.Id,
                RowVersion = p.RowVersion,
                Title = p.Title,
                Price = p.Price,
                Number = p.Number
            });
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