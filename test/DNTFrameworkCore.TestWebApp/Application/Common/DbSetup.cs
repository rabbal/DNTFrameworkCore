using System;
using System.Linq;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Data;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.TestWebApp.Application.Configuration;
using DNTFrameworkCore.TestWebApp.Authorization;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.TestWebApp.Application.Common
{
    public class DbSetup : IDbSetup
    {
        private readonly IDbContext _dbContext;
        private readonly IOptionsSnapshot<ProjectOptions> _settings;
        private readonly IUserPasswordHashAlgorithm _password;
        private readonly ILogger<DbSetup> _logger;

        public DbSetup(IDbContext dbContext,
            IOptionsSnapshot<ProjectOptions> settings,
            IUserPasswordHashAlgorithm password,
            ILogger<DbSetup> logger)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Seed()
        {
            SeedIdentity();
        }

        private void SeedIdentity()
        {
            var role = _dbContext.Set<Role>()
                .Include(r => r.Permissions)
                .FirstOrDefault(r => r.Name == RoleNames.Administrators);
            if (role == null)
            {
                role = new Role
                {
                    Name = RoleNames.Administrators,
                    NormalizedName = RoleNames.Administrators.ToUpperInvariant(),
                    Description =
                        "حذف گروه کاربری پیش فرض «مدیران سیستم» باعث ایجاد اختلال در کارکرد صحیح سیستم خواهد شد.",
                };
                _dbContext.Set<Role>().Add(role);
            }
            else
            {
                _logger.LogInformation($"{nameof(Seed)}: Administrators role already exists.");
            }

            var rolePermissionNames = role.Permissions.Select(a => a.Name).ToList();
            var allPermissionNames = Permissions.Names();

            var newPermissions = allPermissionNames.Except(rolePermissionNames)
                .Select(permissionName => new RolePermission {Name = permissionName}).ToList();
            role.Permissions.AddRange(newPermissions);

            _logger.LogInformation(
                $"{nameof(Seed)}: newPermissions: {string.Join("\n", newPermissions.Select(a => a.Name))}");

            var admin = _settings.Value.UserSeed;

            var user = _dbContext.Set<User>()
                .Include(u => u.Permissions)
                .Include(u => u.Roles)
                .FirstOrDefault(u => u.NormalizedUserName == admin.UserName.ToUpperInvariant());
            if (user == null)
            {
                user = new User
                {
                    UserName = admin.UserName,
                    NormalizedUserName = admin.UserName.ToUpperInvariant(),
                    DisplayName = admin.DisplayName,
                    NormalizedDisplayName = admin.DisplayName, //.NormalizePersianTitle(),
                    IsActive = true,
                    PasswordHash = _password.HashPassword(admin.Password),
                    SecurityToken = Guid.NewGuid().ToString("N")
                };

                _dbContext.Set<User>().Add(user);
            }
            else
            {
                _logger.LogInformation($"{nameof(Seed)}: Admin user already exists.");
            }

            if (user.Roles.All(ur => ur.RoleId != role.Id))
            {
                _dbContext.Set<UserRole>().Add(new UserRole {Role = role, User = user});
            }
            else
            {
                _logger.LogInformation($"{nameof(Seed)}: Admin user already is assigned to Administrators role.");
            }

            user.Permissions.Clear();

            _dbContext.SaveChanges();
        }
    }
}