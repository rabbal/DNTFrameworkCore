using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.Linq;
using DNTFrameworkCore.EFCore.Querying;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Querying;
using DNTFrameworkCore.TestAPI.Application.Identity.Models;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestAPI.Application.Identity
{
    public interface IRoleService : IEntityService<long, RoleReadModel, RoleModel>
    {
    }

    public class RoleService : EntityService<Role, long, RoleReadModel, RoleModel>, IRoleService
    {
        private readonly IMapper _mapper;

        public RoleService(
            IUnitOfWork uow,
            IEventBus bus,
            IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected override IQueryable<Role> FindEntityQueryable =>
            base.FindEntityQueryable.Include(r => r.Permissions);

        public override Task<IPagedResult<RoleReadModel>> FetchPagedListAsync(FilteredPagedRequest request,
            CancellationToken cancellationToken = default)
        {
            return EntitySet.AsNoTracking()
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
        }

        protected override RoleModel MapToModel(Role entity)
        {
            return _mapper.Map<RoleModel>(entity);
        }
    }
}