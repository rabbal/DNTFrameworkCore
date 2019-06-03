using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
    public class
        RolesController : CrudController<IRoleService, long, RoleReadModel, RoleModel, RoleFilteredPagedQueryModel>
    {
        private readonly IPermissionService _permission;
        private readonly IMapper _mapper;
        private readonly IStringLocalizerFactory _localizerFactory;

        public RolesController(
            IRoleService service,
            IPermissionService permission,
            IMapper mapper,
            IStringLocalizerFactory localizerFactory) : base(service)
        {
            _permission = permission ?? throw new ArgumentNullException(nameof(permission));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizerFactory = localizerFactory ?? throw new ArgumentNullException(nameof(localizerFactory));
        }

        protected override string CreatePermissionName => PermissionNames.Roles_Create;
        protected override string EditPermissionName => PermissionNames.Roles_Edit;
        protected override string ViewPermissionName => PermissionNames.Roles_View;
        protected override string DeletePermissionName => PermissionNames.Roles_Delete;
        protected override string ViewName => "_RoleModal";

        protected override IActionResult RenderIndex(IPagedQueryResult<RoleReadModel> model)
        {
            var indexModel = new RoleIndexViewModel
            {
                Items = model.Items,
                TotalCount = model.TotalCount,
                Permissions = ReadPermissionList()
            };

            return Request.IsAjaxRequest()
                ? (IActionResult) PartialView(indexModel)
                : View(indexModel);
        }

        protected override IActionResult RenderView(RoleModel role)
        {
            var model = _mapper.Map<RoleModalViewModel>(role);
            model.PermissionList = ReadPermissionList();

            return PartialView(ViewName, model);
        }

        private List<LookupItem> ReadPermissionList()
        {
            return _permission
                .ReadList()
                .Select(p => new LookupItem {Value = p.Name, Text = p.DisplayName.Localize(_localizerFactory)})
                .ToList();
        }
    }
}