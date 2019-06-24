using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Common
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