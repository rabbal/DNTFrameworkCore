using System;
using System.Linq;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.TestWebApp.Application.Identity;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.TestWebApp.Models.Roles;
using DNTFrameworkCore.Web.Extensions;
using DNTFrameworkCore.Web.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace DNTFrameworkCore.TestWebApp.Controllers
{
    public class RolesController : CrudController<IRoleService, long, RoleReadModel, RoleModel, RoleFilteredPagedQueryModel>
    {
        private readonly IPermissionService _permissionService;
        private readonly IStringLocalizerFactory _localizerFactory;

        public RolesController(
            IRoleService service,
            IPermissionService permissionService,
            IStringLocalizerFactory localizerFactory) : base(service)
        {
            _permissionService = permissionService ?? throw new ArgumentNullException(nameof(permissionService));
            _localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
        }

        protected override string CreatePermissionName => PermissionNames.Roles_Create;
        protected override string EditPermissionName => PermissionNames.Roles_Edit;
        protected override string ViewPermissionName => PermissionNames.Roles_View;
        protected override string DeletePermissionName => PermissionNames.Roles_Delete;
        protected override string ViewName => "_RoleModal";

        protected override IActionResult RenderIndex(PagedListModel<RoleReadModel, RoleFilteredPagedQueryModel> model)
        {
            var indexModel = new RoleIndexViewModel
            {
                PagedList = model,
                Permissions = _permissionService
                    .ReadList()
                    .Select(p => new LookupItem { Value = p.Name, Text = p.DisplayName.Localize(_localizerFactory) })
                    .ToList()
            };
            return Request.IsAjaxRequest()
                ? (IActionResult)PartialView(indexModel)
                : View(indexModel);
        }

        protected override IActionResult RenderView(RoleModel model)
        {
            var modalViewModel = new RoleModalViewModel
            {
                Id = model.Id,
                RowVersion = model.RowVersion,
                Name = model.Name,
                Description = model.Description,
                Permissions = model.Permissions,
                //PermissionList=
            };
            return PartialView(ViewName, modalViewModel);
        }
    }
}