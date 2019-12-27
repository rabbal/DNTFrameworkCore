using DNTFrameworkCore.TestTenancy.Application.Blogging;
using DNTFrameworkCore.TestTenancy.Application.Blogging.Models;
using DNTFrameworkCore.TestTenancy.Authorization;

namespace DNTFrameworkCore.TestTenancy.Controllers
{
    [Route("api/[controller]")]
    public class BlogsController : CrudController<IBlogService, int, BlogModel>
    {
        public BlogsController(IBlogService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Blogs_Create;
        protected override string EditPermissionName => PermissionNames.Blogs_Edit;
        protected override string ViewPermissionName => PermissionNames.Blogs_View;
        protected override string DeletePermissionName => PermissionNames.Blogs_Delete;
    }
}