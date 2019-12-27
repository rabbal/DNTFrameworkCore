using DNTFrameworkCore.TestTenancy.Application.Identity;
using DNTFrameworkCore.TestTenancy.Application.Identity.Models;
using DNTFrameworkCore.TestTenancy.Authorization;

namespace DNTFrameworkCore.TestTenancy.Controllers
{
    [Route("api/[controller]")]
    public class RolesController : CrudController<IRoleService, long, RoleReadModel, RoleModel>
    {
        public RolesController(IRoleService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Roles_Create;
        protected override string EditPermissionName => PermissionNames.Roles_Edit;
        protected override string ViewPermissionName => PermissionNames.Roles_View;
        protected override string DeletePermissionName => PermissionNames.Roles_Delete;
    }
}