using DNTFrameworkCore.Web.API;
using Microsoft.AspNetCore.Mvc;
using ProjectName.API.Authorization;
using ProjectName.Application.Identity;
using ProjectName.Application.Identity.Models;

namespace ProjectName.API.Controllers
{
    [Route("api/[controller]")]
    public class RolesController : EntityController<IRoleService, long, RoleReadModel, RoleModel>
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