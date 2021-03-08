using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
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
        EntityController<TEntityService, TModel> : EntityControllerBase<int, TModel, TModel, FilteredPagedRequest>
        where TEntityService : class, IEntityService<int, TModel>
        where TModel : MasterModel<int>, new()
    {
        protected readonly TEntityService EntityService;

        protected EntityController(TEntityService service)
        {
            EntityService = service ?? throw new ArgumentNullException(nameof(service));
        }

        private protected override Task<IPagedResult<TModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken)
        {
            return EntityService.FetchPagedListAsync(request, cancellationToken);
        }

        private protected override Task<Maybe<TModel>> FindAsync(int id, CancellationToken cancellationToken)
        {
            return EntityService.FindAsync(id, cancellationToken);
        }

        private protected override Task<Result> EditAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.EditAsync(model, cancellationToken);
        }

        private protected override Task<TModel> CreateNewAsync(CancellationToken cancellationToken) =>
            EntityService.CreateNewAsync(cancellationToken);

        private protected override Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.CreateAsync(model, cancellationToken);
        }

        private protected override Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.DeleteAsync(model, cancellationToken);
        }

        private protected override async Task<Result> DeleteAsync(IEnumerable<int> ids,
            CancellationToken cancellationToken)
        {
            var models = await EntityService.FindListAsync(ids, cancellationToken);
            return await EntityService.DeleteAsync(models, cancellationToken);
        }
    }

    [Authorize]
    public abstract class
        EntityController<TEntityService, TKey, TModel> : EntityControllerBase<TKey, TModel, TModel, FilteredPagedRequest
        >
        where TEntityService : class, IEntityService<TKey, TModel>
        where TModel : MasterModel<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TEntityService EntityService;

        protected EntityController(TEntityService service)
        {
            EntityService = service ?? throw new ArgumentNullException(nameof(service));
        }

        private protected override Task<IPagedResult<TModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken)
        {
            return EntityService.FetchPagedListAsync(request, cancellationToken);
        }

        private protected override Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken)
        {
            return EntityService.FindAsync(id, cancellationToken);
        }

        private protected override Task<Result> EditAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.EditAsync(model, cancellationToken);
        }

        private protected override Task<TModel> CreateNewAsync(CancellationToken cancellationToken) =>
            EntityService.CreateNewAsync(cancellationToken);

        private protected override Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.CreateAsync(model, cancellationToken);
        }

        private protected override Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.DeleteAsync(model, cancellationToken);
        }

        private protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids,
            CancellationToken cancellationToken)
        {
            var models = await EntityService.FindListAsync(ids, cancellationToken);
            return await EntityService.DeleteAsync(models, cancellationToken);
        }
    }

    [Authorize]
    public abstract class
        EntityController<TEntityService, TKey, TReadModel, TModel> : EntityControllerBase<TKey, TReadModel, TModel,
            FilteredPagedRequest>
        where TEntityService : class, IEntityService<TKey, TReadModel, TModel>
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TEntityService EntityService;

        protected EntityController(TEntityService service)
        {
            EntityService = service ?? throw new ArgumentNullException(nameof(service));
        }

        private protected override Task<IPagedResult<TReadModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken)
        {
            return EntityService.FetchPagedListAsync(request, cancellationToken);
        }

        private protected override Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken)
        {
            return EntityService.FindAsync(id, cancellationToken);
        }

        private protected override Task<Result> EditAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.EditAsync(model, cancellationToken);
        }

        private protected override Task<TModel> CreateNewAsync(CancellationToken cancellationToken) =>
            EntityService.CreateNewAsync(cancellationToken);

        private protected override Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.CreateAsync(model, cancellationToken);
        }

        private protected override Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.DeleteAsync(model, cancellationToken);
        }

        private protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids,
            CancellationToken cancellationToken)
        {
            var models = await EntityService.FindListAsync(ids, cancellationToken);
            return await EntityService.DeleteAsync(models, cancellationToken);
        }
    }

    [Authorize]
    public abstract class
        EntityController<TEntityService, TKey, TReadModel, TModel, TFilteredPagedRequest> :
            EntityControllerBase<TKey, TReadModel, TModel, TFilteredPagedRequest>
        where TEntityService : class, IEntityService<TKey, TReadModel, TModel, TFilteredPagedRequest>
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedRequest : class, IFilteredPagedRequest, new()
        where TKey : IEquatable<TKey>
    {
        protected readonly TEntityService EntityService;

        protected EntityController(TEntityService service)
        {
            EntityService = service ?? throw new ArgumentNullException(nameof(service));
        }

        private protected override Task<IPagedResult<TReadModel>> FetchPagedListAsync(TFilteredPagedRequest request,
            CancellationToken cancellationToken)
        {
            return EntityService.FetchPagedListAsync(request, cancellationToken);
        }

        private protected override Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken)
        {
            return EntityService.FindAsync(id, cancellationToken);
        }

        private protected override Task<Result> EditAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.EditAsync(model, cancellationToken);
        }

        private protected override Task<TModel> CreateNewAsync(CancellationToken cancellationToken) =>
            EntityService.CreateNewAsync(cancellationToken);

        private protected override Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.CreateAsync(model, cancellationToken);
        }

        private protected override Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken)
        {
            return EntityService.DeleteAsync(model, cancellationToken);
        }

        private protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids,
            CancellationToken cancellationToken)
        {
            var models = await EntityService.FindListAsync(ids, cancellationToken);
            return await EntityService.DeleteAsync(models, cancellationToken);
        }
    }

    public abstract class
        EntityControllerBase<TKey, TReadModel, TModel, TFilteredPagedRequest> : Controller
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedRequest : class, IFilteredPagedRequest, new()
        where TKey : IEquatable<TKey>
    {
        private const string ListViewName = "_List";
        private const string ContinueEditingParameterName = "continueEditing";
        private const string ContinueEditingFormName = "continue-editing";

        protected abstract string CreatePermissionName { get; }
        protected abstract string EditPermissionName { get; }
        protected abstract string ViewPermissionName { get; }
        protected abstract string DeletePermissionName { get; }
        protected abstract string ViewName { get; }

        private protected abstract Task<IPagedResult<TReadModel>> FetchPagedListAsync(TFilteredPagedRequest request,
            CancellationToken cancellationToken);

        private protected abstract Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken);
        private protected abstract Task<Result> EditAsync(TModel model, CancellationToken cancellationToken);
        private protected abstract Task<TModel> CreateNewAsync(CancellationToken cancellationToken);
        private protected abstract Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken);
        private protected abstract Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken);
        private protected abstract Task<Result> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken);

        [HttpGet]
        public async Task<IActionResult> Index(TFilteredPagedRequest request, CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            request ??= Factory<TFilteredPagedRequest>.New();
            var model = await FetchPagedListAsync(request, cancellationToken);

            return RenderIndex(model);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [NoResponseCache]
        public async Task<IActionResult> PagedList(TFilteredPagedRequest request,
            CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            request ??= Factory<TFilteredPagedRequest>.New();
            var model = await FetchPagedListAsync(request, cancellationToken);

            return Ok(model);
        }

        [HttpPost]
        [AjaxOnly]
        [ValidateAntiForgeryToken]
        [NoResponseCache]
        public async Task<IActionResult> List(TFilteredPagedRequest request, CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            request ??= Factory<TFilteredPagedRequest>.New();
            var result = await FetchPagedListAsync(request, cancellationToken);

            var model = new PagedListModel<TReadModel, TFilteredPagedRequest>
            {
                Request = request,
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
        public async Task<IActionResult> Create(CancellationToken cancellationToken = default)
        {
            if (!await HasPermission(CreatePermissionName)) return Forbid();

            var model = await CreateNewAsync(cancellationToken) ?? Factory<TModel>.New();
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
            if (result.Failed) return BadRequest(result);

            ModelState.Clear();
            return continueEditing ? RenderView(model) : Ok();
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
            if (result.Failed) return BadRequest(result);

            ModelState.Clear();
            return continueEditing ? RenderView(model) : Ok();
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
            return !result.Failed ? Ok() : BadRequest(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AjaxOnly]
        [NoResponseCache]
        public async Task<IActionResult> BulkDelete(IEnumerable<TKey> ids, CancellationToken cancellationToken)
        {
            if (!await HasPermission(DeletePermissionName))
            {
                return Forbid();
            }

            var result = await DeleteAsync(ids, cancellationToken);

            return !result.Failed ? NoContent() : BadRequest(result);
        }

        protected async Task<bool> HasPermission(string permissionName)
        {
            var policyName = PermissionConstant.PolicyPrefix + permissionName;
            var authorization = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            return (await authorization.AuthorizeAsync(User, policyName)).Succeeded;
        }

        protected IActionResult BadRequest(Result result)
        {
            ModelState.AddResult(result);
            return BadRequest(ModelState);
        }
    }
}