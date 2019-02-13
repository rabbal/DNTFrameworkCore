using DNTFrameworkCore.TestAPI.Application.Identity;
using DNTFrameworkCore.TestAPI.Application.Identity.Models;
using DNTFrameworkCore.TestAPI.Authorization;
using DNTFrameworkCore.Web.API;
using Microsoft.AspNetCore.Mvc;

namespace DNTFrameworkCore.TestAPI.Controllers
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