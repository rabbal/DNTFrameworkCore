using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.TestTenancy.Domain.Identity;

namespace DNTFrameworkCore.TestTenancy.Application.Common
{
    public interface ILookupService : IScopedDependency
    {
        Task<IReadOnlyList<LookupItem<long>>> ReadRolesAsync();
    }

    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _uow;

        public LookupService(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        }

        public async Task<IReadOnlyList<LookupItem<long>>> ReadRolesAsync()
        {
            var roles = await _uow.Set<Role>().AsNoTracking().Select(role => new LookupItem<long>
            {
                Text = role.Name,
                Value = role.Id
            }).ToListAsync();

            return roles.AsReadOnly();
        }
    }
}