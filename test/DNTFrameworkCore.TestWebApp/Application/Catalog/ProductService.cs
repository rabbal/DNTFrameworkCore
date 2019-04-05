using System.Linq;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EntityFramework.Application;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Mapping;
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
        public ProductService(IUnitOfWork uow, IEventBus bus) : base(uow, bus)
        {
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

        protected override Product MapToEntity(ProductModel model)
        {
            var product = Factory<Product>.CreateInstance();

            product.Id = model.Id;
            product.RowVersion = model.RowVersion;
            product.Title = model.Title;
            product.Number = model.Number;
            product.Price = model.Price;

            return product;
        }

        protected override ProductModel MapToModel(Product entity)
        {
            var model = Factory<ProductModel>.CreateInstance();

            model.Id = entity.Id;
            model.RowVersion = entity.RowVersion;
            model.Title = entity.Title;
            model.Number = entity.Number;
            model.Price = entity.Price;

            return model;
        }
    }
}