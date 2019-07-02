using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.Web.Authorization;
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

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await Service.FindListAsync(ids);
            return await Service.DeleteAsync(models);
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

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await Service.FindListAsync(ids);
            return await Service.DeleteAsync(models);
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

        protected override async Task<Result> DeleteAsync(IEnumerable<TKey> ids)
        {
            var models = await Service.FindListAsync(ids);
            return await Service.DeleteAsync(models);
        }
    }

    [ApiController]
    [Produces("application/json")]
    public abstract class
        CrudControllerBase<TKey, TReadModel, TModel, TFilteredPagedQueryModel> : ControllerBase
        where TReadModel : ReadModel<TKey>
        where TModel : MasterModel<TKey>, new()
        where TFilteredPagedQueryModel : class, IFilteredPagedQueryModel, new()
        where TKey : IEquatable<TKey>
    {
        private IAuthorizationService AuthorizationService =>
            HttpContext.RequestServices.GetRequiredService<IAuthorizationService>();

        protected abstract string CreatePermissionName { get; }
        protected abstract string EditPermissionName { get; }
        protected abstract string ViewPermissionName { get; }
        protected abstract string DeletePermissionName { get; }

        protected abstract Task<IPagedQueryResult<TReadModel>> ReadPagedListAsync(TFilteredPagedQueryModel query);
        protected abstract Task<Maybe<TModel>> FindAsync(TKey id);
        protected abstract Task<Result> EditAsync(TModel model);
        protected abstract Task<Result> CreateAsync(TModel model);
        protected abstract Task<Result> DeleteAsync(TModel model);
        protected abstract Task<Result> DeleteAsync(IEnumerable<TKey> ids);

        [HttpGet]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Get(TFilteredPagedQueryModel query)
        {
            if (!await CheckPermissionAsync(ViewPermissionName)) return Forbid();

            var result = await ReadPagedListAsync(query ?? Factory<TFilteredPagedQueryModel>.CreateInstance());

            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<ActionResult<TModel>> Get([BindRequired] TKey id)
        {
            if (!await CheckPermissionAsync(EditPermissionName)) return Forbid();

            var model = await FindAsync(id);

            return model.HasValue ? (ActionResult) Ok(model.Value) : NotFound();
        }

        [HttpPost]
        [ProducesResponseType((int) HttpStatusCode.Created)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<ActionResult<TModel>> Post(TModel model)
        {
            if (!await CheckPermissionAsync(CreatePermissionName)) return Forbid();

            var result = await CreateAsync(model);
            if (!result.Failed) return Created("", model);

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [ProducesResponseType((int) HttpStatusCode.OK)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Put([BindRequired] TKey id, TModel model)
        {
            if (!model.Id.Equals(id)) return BadRequest();

            if (!await CheckPermissionAsync(EditPermissionName)) return Forbid();

            model.Id = id;

            var result = await EditAsync(model);
            if (!result.Failed) return Ok(model);

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.BadRequest)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        [ProducesResponseType((int) HttpStatusCode.NotFound)]
        public async Task<IActionResult> Delete([BindRequired] TKey id)
        {
            if (!await CheckPermissionAsync(DeletePermissionName)) return Forbid();

            var model = await FindAsync(id);
            if (!model.HasValue) return NotFound();

            var result = await DeleteAsync(model.Value);
            if (!result.Failed) return NoContent();

            ModelState.AddModelError(result);
            return BadRequest(ModelState);
        }

        [HttpPost("[action]")]
        [ProducesResponseType((int) HttpStatusCode.NoContent)]
        [ProducesResponseType((int) HttpStatusCode.Forbidden)]
        public async Task<IActionResult> Delete(IEnumerable<TKey> ids)
        {
            if (!await CheckPermissionAsync(DeletePermissionName))
            {
                return Forbid();
            }

            var result = await DeleteAsync(ids);

            if (!result.Failed)
            {
                return NoContent();
            }

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