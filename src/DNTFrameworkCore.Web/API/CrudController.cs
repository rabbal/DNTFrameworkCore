using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.API
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

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken)
        {
            var models = await Service.FindListAsync(ids, cancellationToken);
            return await Service.DeleteAsync(models, cancellationToken);
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

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken)
        {
            var models = await Service.FindListAsync(ids, cancellationToken);
            return await Service.DeleteAsync(models, cancellationToken);
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

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken)
        {
            var models = await Service.FindListAsync(ids, cancellationToken);
            return await Service.DeleteAsync(models, cancellationToken);
        }
    }

    [ApiController]
    [Produces("application/json")]
    public abstract class
        CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedRequestModel> : ControllerBase
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedRequestModel : class, IFilteredPagedRequest, new()
        where TKey : IEquatable<TKey>
    {
        private IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        protected abstract string CreatePermissionName { get; }
        protected abstract string EditPermissionName { get; }
        protected abstract string ViewPermissionName { get; }
        protected abstract string DeletePermissionName { get; }

        protected abstract Task<IPagedResult<TReadModel>> ReadPagedListAsync(TFilteredPagedRequestModel model,
            CancellationToken cancellationToken);

        protected abstract Task<Maybe<TModel>> FindAsync(TKey id, CancellationToken cancellationToken);
        protected abstract Task<Result> EditAsync(TModel model, CancellationToken cancellationToken);
        protected abstract Task<Result> CreateAsync(TModel model, CancellationToken cancellationToken);
        protected abstract Task<Result> DeleteAsync(TModel model, CancellationToken cancellationToken);
        protected abstract Task<Result> DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken);

        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Get(TFilteredPagedRequestModel model, CancellationToken cancellationToken)
        {
            if (!await HasPermission(ViewPermissionName)) return Forbid();

            var result = await ReadPagedListAsync(model ?? Factory<TFilteredPagedRequestModel>.CreateInstance(),
                cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<TModel>> Get([BindRequired] TKey id, CancellationToken cancellationToken)
        {
            if (!await HasPermission(EditPermissionName)) return Forbid();

            var model = await FindAsync(id, cancellationToken);

            return model.HasValue ? (ActionResult) Ok(model.Value) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<ActionResult<TModel>> Post(TModel model, CancellationToken cancellationToken)
        {
            if (!await HasPermission(CreatePermissionName)) return Forbid();

            var result = await CreateAsync(model, cancellationToken);
            if (!result.Failed) return Created("", model);

            ModelState.AddResult(result);
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Put([BindRequired] TKey id, TModel model, CancellationToken cancellationToken)
        {
            if (!model.Id.Equals(id)) return BadRequest();

            if (!await HasPermission(EditPermissionName)) return Forbid();

            model.Id = id;

            var result = await EditAsync(model, cancellationToken);
            if (!result.Failed) return Ok(model);

            ModelState.AddResult(result);
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete([BindRequired] TKey id, CancellationToken cancellationToken)
        {
            if (!await HasPermission(DeletePermissionName)) return Forbid();

            var model = await FindAsync(id, cancellationToken);
            if (!model.HasValue) return NotFound();

            var result = await DeleteAsync(model.Value, cancellationToken);
            if (!result.Failed) return NoContent();

            ModelState.AddResult(result);
            return BadRequest(ModelState);
        }

        [HttpPost("[action]")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Delete(IEnumerable<TKey> ids, CancellationToken cancellationToken)
        {
            if (!await HasPermission(DeletePermissionName))
            {
                return Forbid();
            }

            var result = await DeleteAsync(ids, cancellationToken);

            if (!result.Failed)
            {
                return NoContent();
            }

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