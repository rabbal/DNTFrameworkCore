using DNTFrameworkCore.TestWebApp.Application.Identity;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.Web.Mvc;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class UsersController : CrudController<IUserService, long, UserReadModel, UserModel>
    {
        public UsersController(IUserService service) : base(service)
        {
        }

        protected override string CreatePermissionName => PermissionNames.Users_Create;
        protected override string EditPermissionName => PermissionNames.Users_Edit;
        protected override string ViewPermissionName => PermissionNames.Users_View;
        protected override string DeletePermissionName => PermissionNames.Users_Delete;
        protected override string ViewName => "_User";
    }
}