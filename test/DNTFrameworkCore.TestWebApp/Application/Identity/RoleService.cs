using System.Linq;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EntityFramework.Application;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using DNTFrameworkCore.TestWebApp.Helpers;
using Microsoft.EntityFrameworkCore;
using DNTFrameworkCore.Linq;
namespace DNTFrameworkCore.TestWebApp.Application.Identity
{
    public interface IRoleService : ICrudService<long, RoleReadModel, RoleModel, RoleFilteredPagedQueryModel>
    {
    }

    public class RoleService : CrudService<Role, long, RoleReadModel, RoleModel, RoleFilteredPagedQueryModel>, IRoleService
    {
        public RoleService(IUnitOfWork uow, IEventBus bus) : base(uow, bus)
        {
        }

        protected override IQueryable<Role> BuildFindQuery()
        {
            return base.BuildFindQuery()
                .Include(r => r.Permissions);
        }

        protected override IQueryable<RoleReadModel> BuildReadQuery(RoleFilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking()
            .WhereIf(
                model.Permissions != null && model.Permissions.Any(),
                r => r.Permissions.Any(p => model.Permissions.Contains(p.Name)))
            .Select(r => new RoleReadModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            });
        }

        protected override Role MapToEntity(RoleModel model)
        {
            return new Role
            {
                Id = model.Id,
                RowVersion = model.RowVersion,
                Name = model.Name,
                NormalizedName = model.Name.NormalizePersianTitle(),
                Description = model.Description,
                Permissions = model.Permissions.Select(p => new RolePermission
                {
                    Id = p.Id,
                    Name = p.Name,
                    TrackingState = p.TrackingState
                }).ToList()
            };
        }

        protected override RoleModel MapToModel(Role entity)
        {
            return new RoleModel
            {
                Id = entity.Id,
                RowVersion = entity.RowVersion,
                Name = entity.Name,
                Description = entity.Description,
                Permissions = entity.Permissions.Select(p => new PermissionModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    TrackingState = p.TrackingState
                }).ToList()
            };
        }
    }
}