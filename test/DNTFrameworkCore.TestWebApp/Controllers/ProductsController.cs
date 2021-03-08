using DNTFrameworkCore.TestWebApp.Application.Catalog;
using DNTFrameworkCore.TestWebApp.Application.Catalog.Models;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.Web.Mvc;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class ProductsController : EntityController<IProductService, long, ProductModel>
    {
        public ProductsController(IProductService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Products_Create;
        protected override string EditPermissionName => PermissionNames.Products_Edit;
        protected override string ViewPermissionName => PermissionNames.Products_View;
        protected override string DeletePermissionName => PermissionNames.Products_Delete;
        protected override string ViewName => "_ProductPartial";
    }
}
