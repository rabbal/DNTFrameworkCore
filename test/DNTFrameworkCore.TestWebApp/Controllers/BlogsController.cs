using DNTFrameworkCore.TestWebApp.Application.Blogging;
using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.Web.Mvc;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class BlogsController : CrudController<IBlogService, int, BlogModel>
    {
        public BlogsController(IBlogService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Blogs_Create;
        protected override string EditPermissionName => PermissionNames.Blogs_Edit;
        protected override string ViewPermissionName => PermissionNames.Blogs_View;
        protected override string DeletePermissionName => PermissionNames.Blogs_Delete;
        protected override string ViewName => "_BlogModal";
    }
}