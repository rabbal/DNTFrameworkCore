using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.Validation;
using DNTFrameworkCore.Web.ExceptionHandling;
using DNTFrameworkCore.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.API
{
    [Authorize]
    public abstract class
        EntityController<TEntityService, TModel> : EntityControllerBase<int, TModel, TModel,
            FilteredPagedRequest>
        where TEntityService : class, IEntityService<int, TModel>
        where TModel : MasterModel<int>
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
        EntityController<TEntityService, TKey, TModel> : EntityControllerBase<TKey, TModel, TModel,
            FilteredPagedRequest>
        where TEntityService : class, IEntityService<TKey, TModel>
        where TModel : MasterModel<TKey>
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
        where TModel : MasterModel<TKey>
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

    [ApiController]
    [Produces("application/json")]
    public abstract class
        EntityControllerBase<TKey, TReadModel, TModel, TFilteredPagedRequest> : ControllerBase
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>
        where TFilteredPagedRequest : class, IFilteredPagedRequest, new()
        where TKey : IEquatable<TKey>
    {
        protected abstract string CreatePermissionName { get; }
        protected abstract string EditPermissionName { get; }
        protected abstract string ViewPermissionName { get; }
        protected abstract string DeletePermissionName { get; }

        private protected abstract Task<IPagedResult<TReadModel>> FetchPagedListAsync(TFilteredPagedRequest request,
            CancellationToken cancellationToken);

        private protected abstract Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken);
        private protected abstract Task<Result> EditAsync(TModel model, CancellationToken cancellationToken);
        private protected abstract Task<TModel> CreateNewAsync(CancellationToken cancellationToken);
        private protected abstract Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken);
        private protected abstract Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken);
        private protected abstract Task<Result> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken);

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Get([FromQuery] TFilteredPagedRequest request,
            CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            var result = await FetchPagedListAsync(request ?? Factory<TFilteredPagedRequest>.New(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("[action]")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TModel>> New(CancellationToken cancellationToken)
        {
            if (!await HasPermission(CreatePermissionName)) return Forbid();

            var model = await CreateNewAsync(cancellationToken);

            return model;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TModel>> Get([BindRequired] TKey id, CancellationToken cancellationToken)
        {
            if (!await HasPermission(EditPermissionName)) return Forbid();

            var model = await FindAsync(id, cancellationToken);

            return !model.HasValue ? null : model.Value;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Post(TModel model, CancellationToken cancellationToken)
        {
            if (!await HasPermission(CreatePermissionName)) return Forbid();

            var result = await CreateAsync(model, cancellationToken);
            return !result.Failed ? Created($"{HttpContext.Request.Path}/{model.Id}", model) : FailureRequest(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<ActionResult<TModel>> Put([BindRequired] TKey id, TModel model,
            CancellationToken cancellationToken)
        {
            if (!model.Id.Equals(id)) return BadRequest();

            if (!await HasPermission(EditPermissionName)) return Forbid();

            model.Id = id;

            var result = await EditAsync(model, cancellationToken);
            return !result.Failed ? model : FailureRequest(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete([BindRequired] TKey id, CancellationToken cancellationToken)
        {
            if (!await HasPermission(DeletePermissionName)) return Forbid();

            var model = await FindAsync(id, cancellationToken);
            if (!model.HasValue) return NotFound();

            var result = await DeleteAsync(model.Value, cancellationToken);
            return !result.Failed ? NoContent() : FailureRequest(result);
        }

        [HttpPost("[action]")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> BulkDelete(IEnumerable<TKey> ids, CancellationToken cancellationToken)
        {
            if (!await HasPermission(DeletePermissionName))
            {
                return Forbid();
            }

            var result = await DeleteAsync(ids, cancellationToken);

            return !result.Failed ? NoContent() : FailureRequest(result);
        }

        protected virtual async Task<bool> HasPermission(string permissionName)
        {
            var policyName = PermissionConstant.PolicyPrefix + permissionName;
            var authorization = HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();
            return (await authorization.AuthorizeAsync(User, policyName)).Succeeded;
        }
        
        protected BadRequestObjectResult FailureRequest()
        {
            return FailureRequest(ModelState);
        }

        protected virtual BadRequestObjectResult FailureRequest(ModelStateDictionary modelState)
        {
            var detail = FailureProblemDetail.FromHttpContext(HttpContext).WithFailures(modelState.ToSerializable());
            return BadRequest(detail);
        }

        protected virtual BadRequestObjectResult FailureRequest(Result result)
        {
            var detail = FailureProblemDetail.FromHttpContext(HttpContext, result.Message)
                .WithFailures(result.Failures);
            return BadRequest(detail);
        }
    }
}