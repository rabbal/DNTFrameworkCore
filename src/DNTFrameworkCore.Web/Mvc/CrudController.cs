using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.Querying;
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
            FilteredPagedRequestModel>
        where TCrudService : class, ICrudService<TKey, TModel>
        where TModel : MasterModel<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected override Task<IPagedResult<TModel>> ReadPagedListAsync(FilteredPagedRequestModel model,
            CancellationToken cancellationToken)
        {
            return Service.ReadPagedListAsync(model, cancellationToken);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken)
        {
            return Service.FindAsync(id, cancellationToken);
        }

        protected override Task<Result> EditAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.EditAsync(model, cancellationToken);
        }

        protected override Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.CreateAsync(model, cancellationToken);
        }

        protected override Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.DeleteAsync(model, cancellationToken);
        }
    }

    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TReadModel, TModel> : CrudControllerBase<TKey, TReadModel, TModel,
            FilteredPagedRequestModel>
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

        protected override Task<IPagedResult<TReadModel>> ReadPagedListAsync(FilteredPagedRequestModel model,
            CancellationToken cancellationToken)
        {
            return Service.ReadPagedListAsync(model, cancellationToken);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken)
        {
            return Service.FindAsync(id, cancellationToken);
        }

        protected override Task<Result> EditAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.EditAsync(model, cancellationToken);
        }

        protected override Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.CreateAsync(model, cancellationToken);
        }

        protected override Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.DeleteAsync(model, cancellationToken);
        }
    }

    [Authorize]
    public abstract class
        CrudController<TCrudService, TKey, TReadModel, TModel, TFilteredPagedRequestModel> :
            CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedRequestModel>
        where TCrudService : class, ICrudService<TKey, TReadModel, TModel, TFilteredPagedRequestModel>
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedRequestModel : class, IFilteredPagedRequest, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TCrudService Service;

        protected CrudController(TCrudService service)
        {
            Service = service ?? throw new ArgumentNullException(nameof(service));
        }

        protected override Task<IPagedResult<TReadModel>> ReadPagedListAsync(TFilteredPagedRequestModel model,
            CancellationToken cancellationToken)
        {
            return Service.ReadPagedListAsync(model, cancellationToken);
        }

        protected override Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken)
        {
            return Service.FindAsync(id, cancellationToken);
        }

        protected override Task<Result> EditAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.EditAsync(model, cancellationToken);
        }

        protected override Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.CreateAsync(model, cancellationToken);
        }

        protected override Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            return Service.DeleteAsync(model, cancellationToken);
        }
    }

    public abstract class
        CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedRequestModel> : Controller
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedRequestModel : class, IFilteredPagedRequest, new()
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

        protected abstract Task<IPagedResult<TReadModel>> ReadPagedListAsync(TFilteredPagedRequestModel model,
            CancellationToken cancellationToken);

        protected abstract Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken);
        protected abstract Task<Result> EditAsync(TModel model, CancellationToken cancellationToken);
        protected abstract Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken);
        protected abstract Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken);

        [HttpGet]
        public async Task<IActionResult> Index(TFilteredPagedRequestModel query, CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            query ??= Factory<TFilteredPagedRequestModel>.CreateInstance();
            var model = await ReadPagedListAsync(query, cancellationToken);
            
            return RenderIndex(model);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [NoResponseCache]
        public async Task<IActionResult> ReadPagedList(TFilteredPagedRequestModel model,
            CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            model ??= Factory<TFilteredPagedRequestModel>.CreateInstance();
            var result = await ReadPagedListAsync(model, cancellationToken);

            return Ok(result);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [NoResponseCache]
        public async Task<IActionResult> List(TFilteredPagedRequestModel query, CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            query = query ?? Factory<TFilteredPagedRequestModel>.CreateInstance();
            var result = await ReadPagedListAsync(query, cancellationToken);

            var model = new PagedListModel<TReadModel, TFilteredPagedRequestModel>
            {
                Request = query,
                Result = result
            };

            return PartialView(ListViewName, model);
        }

        protected virtual IActionResult RenderIndex(IPagedResult<TReadModel> model)
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
            if (!await HasPermission(CreatePermissionName)) return Forbid();

            var model = Factory<TModel>.CreateInstance();
            return RenderView(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [ParameterBasedOnFormName(ContinueEditingFormName, ContinueEditingParameterName)]
        public async Task<IActionResult> Create(TModel model, bool continueEditing, CancellationToken cancellationToken)
        {
            if (!await HasPermission(CreatePermissionName)) return Forbid();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await CreateAsync(model, cancellationToken);
            if (!result.Failed)
            {
                ModelState.Clear();
                return continueEditing ? RenderView(model) : Ok();
            }

            ModelState.AddResult(result);
            return BadRequest(ModelState);
        }

        [HttpGet]
        [NoResponseCache]
        [AjaxOnly]
        public async Task<IActionResult> Edit([BindRequired] TKey id, CancellationToken cancellationToken)
        {
            if (!await HasPermission(EditPermissionName)) return Forbid();

            var model = await FindAsync(id, cancellationToken);

            return !model.HasValue ? NotFound() : RenderView(model.Value);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [ParameterBasedOnFormName(ContinueEditingFormName, ContinueEditingParameterName)]
        public async Task<IActionResult> Edit(TModel model, bool continueEditing, CancellationToken cancellationToken)
        {
            if (!await HasPermission(EditPermissionName)) return Forbid();

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await EditAsync(model, cancellationToken);
            if (!result.Failed)
            {
                ModelState.Clear();
                return continueEditing ? RenderView(model) : Ok();
            }

            ModelState.AddResult(result);
            return BadRequest(ModelState);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [NoResponseCache]
        public async Task<IActionResult> Delete([BindRequired] TKey id, CancellationToken cancellationToken)
        {
            if (!await HasPermission(DeletePermissionName)) return Forbid();

            var model = await FindAsync(id, cancellationToken);
            if (!model.HasValue) return NotFound();

            var result = await DeleteAsync(model.Value, cancellationToken);
            if (!result.Failed) return Ok();

            ModelState.AddResult(result);
            return BadRequest(ModelState);
        }

        private async Task<bool> HasPermission(string permissionName)
        {
            var policyName = PermissionConstant.PolicyPrefix + permissionName;
            return (await AuthorizationService.AuthorizeAsync(User, policyName)).Succeeded;
        }
    }
}