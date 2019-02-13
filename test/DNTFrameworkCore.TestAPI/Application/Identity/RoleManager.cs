using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestAPI.Application.Identity
{
    public interface IRoleManager : ITransientDependency
    {
        Task<IList<Role>> FindUserRolesAsync(long userId);
        Task<IList<Role>> FindUserRolesIncludeClaimsAsync(long userId);
        Task<bool> IsUserInRole(long userId, string roleName);
        Task<bool> IsUserInRole(long userId, long roleId);
        Task<IList<User>> FindUsersInRoleAsync(string roleName);
        Task<IList<User>> FindUsersInRoleAsync(long roleId);
    }

    public class RoleManager : IRoleManager
    {
        private readonly IUnitOfWork _uow;
        private readonly DbSet<Role> _roles;
        private readonly DbSet<User> _users;

        public RoleManager(IUnitOfWork uow)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));

            _roles = _uow.Set<Role>();
            _users = _uow.Set<User>();
        }

        public async Task<IList<Role>> FindUserRolesAsync(long userId)
        {
            var userRolesQuery = from role in _roles
                from userRoles in role.Users
                where userRoles.UserId == userId
                select role;

            return await userRolesQuery
                .AsNoTracking()
                .OrderBy(x => x.Name).ToListAsync();
        }

        public async Task<IList<Role>> FindUserRolesIncludeClaimsAsync(long userId)
        {
            var userRolesQuery = from role in _roles
                from userRoles in role.Users
                where userRoles.UserId == userId
                select role;

            return await userRolesQuery
                .AsNoTracking()
                .Include(r => r.Permissions)
                .Include(r => r.Claims)
                .OrderBy(x => x.Name).ToListAsync();
        }

        public Task<bool> IsUserInRole(long userId, string roleName)
        {
            var userRolesQuery = from role in _roles
                where role.Name == roleName
                from user in role.Users
                where user.UserId == userId
                select role;

            return userRolesQuery
                .AsNoTracking()
                .AnyAsync();
        }

        public Task<bool> IsUserInRole(long userId, long roleId)
        {
            return _users
                .AsNoTracking()
                .AnyAsync(user => user.Id == userId && user.Roles.Any(ur => ur.RoleId == roleId));
        }

        public async Task<IList<User>> FindUsersInRoleAsync(string roleName)
        {
            var roleUserIdsQuery = from role in _roles
                where role.Name == roleName
                from user in role.Users
                select user.UserId;

            return await _users.AsNoTracking()
                .Where(user => roleUserIdsQuery.Contains(user.Id))
                .ToListAsync();
        }

        public async Task<IList<User>> FindUsersInRoleAsync(long roleId)
        {
            return await _users.AsNoTracking()
                .Where(user => user.Roles.Any(ur => ur.RoleId == roleId)).ToListAsync();
        }
    }
}