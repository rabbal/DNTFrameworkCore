using System;
using System.Linq;
using DNTFrameworkCore.Authorization;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.Data;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.TestAPI.Application.Configuration;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.TestAPI.Application.Common
{
    public class DbSeed : IDbSeed
    {
        private readonly IUnitOfWork _uow;
        private readonly IOptionsSnapshot<ProjectSettings> _settings;
        private readonly IUserPassword _password;
        private readonly IPermissionService _permissionManager;
        private readonly ILogger<DbSeed> _logger;

        public DbSeed(IUnitOfWork uow,
            IOptionsSnapshot<ProjectSettings> settings,
            IUserPassword password,
            IPermissionService permissionManager,
            ILogger<DbSeed> logger)
        {
            _uow = uow ?? throw new ArgumentNullException(nameof(uow));
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _password = password ?? throw new ArgumentNullException(nameof(password));
            _permissionManager = permissionManager ?? throw new ArgumentNullException(nameof(permissionManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Seed()
        {
            SeedIdentity();
        }

        private void SeedIdentity()
        {
            var role = _uow.Set<Role>()
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
                _uow.Set<Role>().Add(role);
            }
            else
            {
                _logger.LogInformation($"{nameof(Seed)}: Administrators role already exists.");
            }

            var rolePermissionNames = role.Permissions.Select(a => a.Name).ToList();
            var allPermissionNames = _permissionManager.ReadList().Select(p => p.Name);

            var newPermissions = allPermissionNames.Except(rolePermissionNames)
                .Select(permissionName => new RolePermission {Name = permissionName}).ToList();
            role.Permissions.AddRange(newPermissions);

            _logger.LogInformation(
                $"{nameof(Seed)}: newPermissions: {string.Join("\n", newPermissions.Select(a => a.Name))}");

            var admin = _settings.Value.UserSeed;

            var user = _uow.Set<User>()
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
                    SerialNumber = Guid.NewGuid().ToString("N")
                };

                _uow.Set<User>().Add(user);
            }
            else
            {
                _logger.LogInformation($"{nameof(Seed)}: Admin user already exists.");
            }

            if (user.Roles.All(ur => ur.RoleId != role.Id))
            {
                _uow.Set<UserRole>().Add(new UserRole {Role = role, User = user});
            }
            else
            {
                _logger.LogInformation($"{nameof(Seed)}: Admin user already is assigned to Administrators role.");
            }

            user.Permissions.Clear();

            _uow.SaveChanges();
        }
    }
}