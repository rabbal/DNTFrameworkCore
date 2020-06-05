using DNTFrameworkCore.TestWebApp.Application.Identity;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.Web.Mvc;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class RolesController :
        CrudController<IRoleService, long, RoleReadModel, RoleModel, RoleFilteredPagedRequest>
    {
        public RolesController(IRoleService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Roles_Create;
        protected override string EditPermissionName => PermissionNames.Roles_Edit;
        protected override string ViewPermissionName => PermissionNames.Roles_View;
        protected override string DeletePermissionName => PermissionNames.Roles_Delete;
        protected override string ViewName => "_RolePartial";
    }
}