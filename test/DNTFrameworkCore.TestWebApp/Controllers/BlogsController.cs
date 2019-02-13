//using System;
//using System.Threading.Tasks;
//using DNTFrameworkCore.Application.Models;
//using DNTFrameworkCore.TestWebApp.Application.Blogging;
//using DNTFrameworkCore.TestWebApp.Application.Blogging.Models;
//using DNTFrameworkCore.TestWebApp.Authorization;
//using DNTFrameworkCore.Web.ActionSelectors;
//using DNTFrameworkCore.Web.Authorization;
//using DNTFrameworkCore.Web.Extensions;
//using DNTFrameworkCore.Web.Mvc.Controllers;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//
//namespace DNTFrameworkCore.TestWebApp.Controllers
//{
//    public class BlogsController : CrudController
//    {
//        private readonly IBlogService _service;
//        private readonly IAuthorizationService _authService;
//
//        protected override string CreatePermissionName => PermissionNames.Administration_Blogs_Create;
//        protected override string EditPermissionName => PermissionNames.Administration_Blogs_Edit;
//        protected override string ViewPermissionName => PermissionNames.Administration_Blogs_View;
//        protected override string DeletePermissionName => PermissionNames.Administration_Blogs_Delete;
//
//        public BlogsController(IBlogService service, IAuthorizationService authService) : base(authService)
//        {
//            _service = service ?? throw new ArgumentNullException(nameof(service));
//            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
//        }
//
//        [HttpGet]
//        public async Task<IActionResult> List(FilteredPagedQueryModel query)
//        {
//            var result = await _service.ReadPagedListAsync(query ?? new FilteredPagedQueryModel());
//            var model = new PagedListModel<BlogModel, FilteredPagedQueryModel>
//            {
//                Query = query,
//                Result = result
//            };
//
//            if (Request.IsAjaxRequest())
//            {
//                return PartialView("List.cshtml", model);
//            }
//
//            return View(model);
//        }
//
//
//        [HttpGet]
//        public virtual async Task<ActionResult> Index()
//        {
//            var query = new RolePagedQueryModel();
//            var result = await _service.ReadPagedListAsync(query);
//
//            var pagedList = new PagedListModel<BlogModel>
//            {
//                Query = query,
//                Result = result
//            };
//
//            var model = new RoleIndexViewModel
//            {
//                PagedListModel = pagedList,
//                Permissions = _lookupService.GetPermissions()
//            };
//            return View(model);
//        }
//
//        [MvcAuthorize(PermissionNames.Pages_Administration_Roles)]
//        [HttpGet, AjaxOnly, NoOutputCache]
//        public virtual async Task<ActionResult> List(RolePagedQueryModel query)
//        {
//            var result = await _service.GetPagedListAsync(query);
//
//            var model = new PagedListModel<RoleModel>
//            {
//                Query = query,
//                Result = result
//            };
//
//            return PartialView(MVC.Administration.Roles.Views._List, model);
//        }
//
//        [MvcAuthorize(PermissionNames.Pages_Administration_Roles_Create)]
//        [HttpGet, AjaxOnly]
//        public virtual ActionResult Create()
//        {
//            var model = new RoleCreateModalViewModel
//            {
//                Permissions = _permissionService.GetHierarchicalList()
//            };
//            return PartialView(MVC.Administration.Roles.Views._Create, model);
//        }
//
//        [MvcAuthorize(PermissionNames.Pages_Administration_Roles_Create)]
//        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
//        public virtual async Task<ActionResult> Create([Bind(Prefix = "Model")]RoleCreateModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return PartialView(MVC.Administration.Roles.Views._Create, new RoleCreateModalViewModel
//                {
//                    Model = model,
//                    Permissions = _permissionService.GetHierarchicalList(model.Permissions)
//                });
//            }
//
//            await _service.CreateAsync(model);
//
//            return InformationNotification("Messages.Save.Success");
//        }
//
//        [MvcAuthorize(PermissionNames.Pages_Administration_Roles_Edit)]
//        [HttpGet, AjaxOnly, NoOutputCache]
//        public virtual async Task<ActionResult> Edit(long id)
//        {
//            var editModel = await _service.GetForEditAsync(id);
//
//            var model = new RoleEditModalViewModel
//            {
//                Model = editModel,
//                Permissions = _permissionService.GetHierarchicalList(editModel.Permissions)
//            };
//
//            return PartialView(MVC.Administration.Roles.Views._Edit, model);
//        }
//
//        [MvcAuthorize(PermissionNames.Pages_Administration_Roles_Edit)]
//        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
//        public virtual async Task<ActionResult> Edit([Bind(Prefix = "Model")]RoleEditModel model)
//        {
//            if (!ModelState.IsValid)
//            {
//                return PartialView(MVC.Administration.Roles.Views._Edit, new RoleEditModalViewModel
//                {
//                    Model = model,
//                    Permissions = _permissionService.GetHierarchicalList(model.Permissions)
//                });
//            }
//
//            await _service.EditAsync(model);
//
//            return InformationNotification("Messages.Save.Success");
//        }
//
//        [HttpPost, ValidateAntiForgeryToken, AjaxOnly]
//        public virtual async Task<ActionResult> Delete(BlogModel model)
//        {
//            var authResult = await CheckPermissionAsync()
//            await _service.DeleteAsync(model);
//
//            return SuccessNotification("Messages.Delete.Success");
//        }
//
//
//        private Task<AuthorizationResult> CheckPermissionAsync(string permissionName)
//        {
//            return _authService.AuthorizeAsync(User, BuildPolicyName(permissionName));
//        }
//
//        private static string BuildPolicyName(string permission)
//        {
//            return "PermissionAuthorizeAttribute.PolicyPrefix + permission";
//        }
//    }
//}