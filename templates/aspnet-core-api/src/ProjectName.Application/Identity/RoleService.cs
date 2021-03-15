using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using Microsoft.EntityFrameworkCore;
using ProjectName.Application.Identity.Models;
using ProjectName.Domain.Identity;

namespace ProjectName.Application.Identity
{
    public interface IRoleService : IEntityService<long, RoleReadModel, RoleModel>
    {
    }

    public class RoleService : EntityService<Role, long, RoleReadModel, RoleModel>, IRoleService
    {
        private readonly IMapper _mapper;

        public RoleService(IDbContext dbContext, IEventBus bus, IMapper mapper) : base(dbContext, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override IQueryable<Role> FindEntityQueryable => base.FindEntityQueryable
            .Include(r => r.Permissions);

        public override Task<IPagedResult<RoleReadModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking().Select(r => new RoleReadModel
            {
                Id = r.Id,
                Name = r.Name,
                Description = r.Description
            }).ToPagedListAsync(request, cancellationToken: cancellationToken);
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