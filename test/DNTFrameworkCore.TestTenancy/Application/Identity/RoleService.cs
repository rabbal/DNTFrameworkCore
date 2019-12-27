using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.TestTenancy.Application.Identity.Models;
using DNTFrameworkCore.TestTenancy.Domain.Identity;

namespace DNTFrameworkCore.TestTenancy.Application.Identity
{
    public interface IRoleService : ICrudService<long, RoleReadModel, RoleModel>
    {
    }

    public class RoleService : CrudService<Role, long, RoleReadModel, RoleModel>, IRoleService
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

        protected override IQueryable<RoleReadModel> BuildReadQuery(FilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking()
                .Select(r => new RoleReadModel
                {
                    Id = r.Id,
                    Name = r.Name,
                    Description = r.Description
                });
        }

        protected override Task<Result> BeforeEditAsync(IReadOnlyList<ModifiedModel<RoleModel>> models,
            IReadOnlyList<Role> entities)
        {
            return base.BeforeEditAsync(models, entities);
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