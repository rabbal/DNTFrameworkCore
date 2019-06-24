using System.Linq;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using DNTFrameworkCore.Linq;
using AutoMapper;
using System;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;

namespace DNTFrameworkCore.TestWebApp.Application.Identity
{
    public interface IRoleService : ICrudService<long, RoleReadModel, RoleModel, RoleFilteredPagedQueryModel>
    {
    }

    public class RoleService : CrudService<Role, long, RoleReadModel, RoleModel, RoleFilteredPagedQueryModel>,
        IRoleService
    {
        private readonly IMapper _mapper;

        public RoleService(
            IUnitOfWork uow,
            IEventBus bus,
            IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override IQueryable<Role> BuildFindQuery()
        {
            return base.BuildFindQuery()
                .Include(r => r.Permissions);
        }

        protected override IQueryable<RoleReadModel> BuildReadQuery(RoleFilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking()
                .WhereIf(model.Permissions != null && model.Permissions.Any(),
                    r => r.Permissions.Any(p => model.Permissions.Contains(p.Name)))
                .Select(r => new RoleReadModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                });
        }

        protected override void MapToEntity(RoleModel model, Role role)
        {
            _mapper.Map(model, role);
        }

        protected override RoleModel MapToModel(Role role)
        {
            return _mapper.Map<RoleModel>(role);
        }
    }
}