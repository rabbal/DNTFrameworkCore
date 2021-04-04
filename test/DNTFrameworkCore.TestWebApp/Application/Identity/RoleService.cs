using System.Linq;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using DNTFrameworkCore.Linq;
using AutoMapper;
using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Querying;

namespace DNTFrameworkCore.TestWebApp.Application.Identity
{
    public interface IRoleService : IEntityService<long, RoleReadModel, RoleModel, RoleFilteredPagedRequest>
    {
    }

    public class RoleService :
        EntityService<Role, long, RoleReadModel, RoleModel, RoleFilteredPagedRequest>,
        IRoleService
    {
        private readonly IMapper _mapper;

        public RoleService(
            IDbContext dbContext,
            IEventBus bus,
            IMapper mapper) : base(dbContext, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override IQueryable<Role> FindEntityQueryable => base.FindEntityQueryable.Include(r => r.Permissions);

        public override Task<IPagedResult<RoleReadModel>> FetchPagedListAsync(RoleFilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking()
                .WhereIf(request.Permissions != null && request.Permissions.Any(),
                    r => r.Permissions.Any(p => request.Permissions.Contains(p.Name)))
                .Select(r => new RoleReadModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                }).ToPagedListAsync(request, cancellationToken);
        }

        protected override void MapToEntity(RoleModel model, Role role)
        {
            _mapper.Map(model, role);
            MapPermissions(model, role);
        }

        private static void MapPermissions(RoleModel model, Role role)
        {
            var addedPermissions = model.PermissionNames.Where(permissionName =>
                    !role.Permissions.Select(_ => _.Name).Contains(permissionName))
                .Select(permissionName => new RolePermission
                {
                    Name = permissionName,
                    IsGranted = true,
                    TrackingState = TrackingState.Added
                });
            role.Permissions.AddRange(addedPermissions);

            var removedPermissions = role.Permissions.Where(p => !model.PermissionNames.Contains(p.Name));
            foreach (var removedPermission in removedPermissions)
            {
                removedPermission.TrackingState = TrackingState.Deleted;
            }
        }

        protected override RoleModel MapToModel(Role role)
        {
            return _mapper.Map<RoleModel>(role);
        }
    }
}