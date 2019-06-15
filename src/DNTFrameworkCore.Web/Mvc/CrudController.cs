using System;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.Web.Authorization;
using DNTFrameworkCore.Web.Extensions;
using DNTFrameworkCore.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Mvc
{
    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TModel> : CrudControllerBase<TKey, TModel, TModel,
            FilteredPagedQueryModel>
        where TCrudService : class, ICrudService<TKey, TModel>
        where TModel : MasterModel<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected override Task<IPagedQueryResult<TModel>> ReadPagedListAsync(FilteredPagedQueryModel query)
        {
            return Service.ReadPagedListAsync(query);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id)
        {
            return Service.FindAsync(id);
        }

        protected override Task<Result> EditAsync(TModel model)
        {
            return Service.EditAsync(model);
        }

        protected override Task<Result> CreateAsync(TModel model)
        {
            return Service.CreateAsync(model);
        }

        protected override Task<Result> DeleteAsync(TModel model)
        {
            return Service.DeleteAsync(model);
        }
    }

    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TReadModel, TModel> : CrudControllerBase<TKey, TReadModel, TModel,
            FilteredPagedQueryModel>
        where TCrudService : class, ICrudService<TKey, TReadModel, TModel>
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected override Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(FilteredPagedQueryModel query)
        {
            return Service.ReadPagedListAsync(query);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id)
        {
            return Service.FindAsync(id);
        }

        protected override Task<Result> EditAsync(TModel model)
        {
            return Service.EditAsync(model);
        }

        protected override Task<Result> CreateAsync(TModel model)
        {
            return Service.CreateAsync(model);
        }

        protected override Task<Result> DeleteAsync(TModel model)
        {
            return Service.DeleteAsync(model);
        }
    }

    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TReadModel, TModel, TFilteredPagedQueryModel> :
            CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedQueryModel>
        where TCrudService : class, ICrudService<TKey, TReadModel, TModel, TFilteredPagedQueryModel>
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected override Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel query)
        {
            return Service.ReadPagedListAsync(query);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id)
        {
            return Service.FindAsync(id);
        }

        protected override Task<Result> EditAsync(TModel model)
        {
            return Service.EditAsync(model);
        }

        protected override Task<Result> CreateAsync(TModel model)
        {
            return Service.CreateAsync(model);
        }

        protected override Task<Result> DeleteAsync(TModel model)
        {
            return Service.DeleteAsync(model);
        }
    }

    public abstract class
        CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedQueryModel> : Controller
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel, new()
        where TKey : IEquatable<TKey>
    {
        private const string ListViewName = "_List";
        private const string ContinueEditingParameterName = "continueEditing";
        private const string ContinueEditingFormName = "save-continue";

        private IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        protected abstract string CreatePermissionName { get; }
        protected abstract string EditPermissionName { get; }
        protected abstract string ViewPermissionName { get; }
        protected abstract string DeletePermissionName { get; }
        protected abstract string ViewName { get; }

        protected abstract Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel query);
        protected abstract Task<Maybe<TModel>> FindAsync(TKey id);
        protected abstract Task<Result> EditAsync(TModel model);
        protected abstract Task<Result> CreateAsync(TModel model);
        protected abstract Task<Result> DeleteAsync(TModel model);

        [HttpGet]
        public async Task<IActionResult> Index(TFilteredPagedQueryModel query)
        {
            if (!await CheckPermissionAsync(ViewPermissionName)) return Forbid();

            query = query ?? Factory<TFilteredPagedQueryModel>.CreateInstance();
            var model = await ReadPagedListAsync(query);

            return RenderIndex(model);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [NoResponseCache]
        public async Task<IActionResult> ReadPagedList(TFilteredPagedQueryModel query)
        {
            if (!await CheckPermissionAsync(ViewPermissionName)) return Forbid();

            query = query ?? Factory<TFilteredPagedQueryModel>.CreateInstance();
            var result = await ReadPagedListAsync(query);

            return Ok(result);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [NoResponseCache]
        public async Task<IActionResult> List(TFilteredPagedQueryModel query)
        {
            if (!await CheckPermissionAsync(ViewPermissionName)) return Forbid();

            query = query ?? Factory<TFilteredPagedQueryModel>.CreateInstance();
            var result = await ReadPagedListAsync(query);

            var model = new PagedListModel<TReadModel, TFilteredPagedQueryModel>
            {
                Query = query,
                Result = result
            };

            return PartialView(ListViewName, model);
        }

        protected virtual IActionResult RenderIndex(IPagedQueryResult<TReadModel> model)
        {
            return Request.IsAjaxRequest()
                ? (IActionResult) PartialView(model)
                : View(model);
        }

        protected virtual IActionResult RenderView(TModel model)
        {
            return PartialView(ViewName, model);
        }

        [HttpGet]
        [NoResponseCache]
        [AjaxOnly]
        public async Task<IActionResult> Create()
        {
            if (!await CheckPermissionAsync(CreatePermissionName)) return Forbid();

            var model = Factory<TModel>.CreateInstance();
            return RenderView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [ParameterBasedOnFormName(ContinueEditingFormName, ContinueEditingParameterName)]
        public async Task<IActionResult> Create(TModel model, bool continueEditing)
        {
            if (!await CheckPermissionAsync(CreatePermissionName)) return Forbid();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await CreateAsync(model);
            if (!result.Failed)
            {
                ModelState.Clear();
                return continueEditing ? RenderView(model) : Ok();
            }

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpGet]
        [NoResponseCache]
        [AjaxOnly]
        public async Task<IActionResult> Edit([BindRequired] TKey id)
        {
            if (!await CheckPermissionAsync(EditPermissionName)) return Forbid();

            var model = await FindAsync(id);

            return !model.HasValue ? NotFound() : RenderView(model.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [ParameterBasedOnFormName(ContinueEditingFormName, ContinueEditingParameterName)]
        public async Task<IActionResult> Edit(TModel model, bool continueEditing)
        {
            if (!await CheckPermissionAsync(EditPermissionName)) return Forbid();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await EditAsync(model);
            if (!result.Failed)
            {
                ModelState.Clear();
                return continueEditing ? RenderView(model) : Ok();
            }

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [NoResponseCache]
        public async Task<IActionResult> Delete([BindRequired] TKey id)
        {
            if (!await CheckPermissionAsync(DeletePermissionName)) return Forbid();

            var model = await FindAsync(id);
            if (!model.HasValue) return NotFound();

            var result = await DeleteAsync(model.Value);
            if (!result.Failed) return Ok();

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        private async Task<bool> CheckPermissionAsync(string permissionName)
        {
            return (await AuthorizationService.AuthorizeAsync(User, BuildPolicyName(permissionName))).Succeeded;
        }

        private static string BuildPolicyName(string permission)
        {
            return PermissionAuthorizeAttribute.PolicyPrefix + permission;
        }
    }
}