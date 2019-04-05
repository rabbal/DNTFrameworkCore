using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.EntityFramework.Application;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.TestWebApp.Application.Identity.Models;
using DNTFrameworkCore.TestWebApp.Domain.Identity;
using DNTFrameworkCore.TestWebApp.Helpers;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestWebApp.Application.Identity
{
    public interface IUserService : ICrudService<long, UserReadModel, UserModel>
    {
    }

    public class UserService : CrudService<User, long, UserReadModel, UserModel>, IUserService
    {
        private readonly IUserManager _manager;

        public UserService(IUnitOfWork uow, IEventBus bus, IUserManager manager) : base(uow, bus)
        {
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        protected override IQueryable<User> BuildFindQuery()
        {
            return base.BuildFindQuery()
                .Include(u => u.Roles)
                .Include(u => u.Permissions);
        }

        protected override IQueryable<UserReadModel> BuildReadQuery(FilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking().Select(u => new UserReadModel
            {
                Id = u.Id,
                IsActive = u.IsActive,
                UserName = u.UserName,
                DisplayName = u.DisplayName,
                LastLoggedInDateTime = u.LastLoggedInDateTime
            });
        }

        protected override User MapToEntity(UserModel model)
        {
            return new User
            {
                Id = model.Id,
                RowVersion = model.RowVersion,
                IsActive = model.IsActive,
                DisplayName = model.DisplayName,
                UserName = model.UserName,
                NormalizedUserName = model.UserName.ToUpperInvariant(),
                NormalizedDisplayName = model.DisplayName.NormalizePersianTitle(),
                Roles = model.Roles.Select(r => new UserRole
                { Id = r.Id, RoleId = r.RoleId, TrackingState = r.TrackingState }).ToList(),
                Permissions = model.Permissions.Select(p => new UserPermission
                {
                    Id = p.Id,
                    TrackingState = p.TrackingState,
                    IsGranted = true,
                    Name = p.Name
                }).Union(model.IgnoredPermissions.Select(p => new UserPermission
                {
                    Id = p.Id,
                    TrackingState = p.TrackingState,
                    IsGranted = false,
                    Name = p.Name
                })).ToList()
            };
        }

        protected override UserModel MapToModel(User entity)
        {
            return new UserModel
            {
                Id = entity.Id,
                RowVersion = entity.RowVersion,
                IsActive = entity.IsActive,
                DisplayName = entity.DisplayName,
                UserName = entity.UserName,
                Roles = entity.Roles.Select(r => new UserRoleModel
                { Id = r.Id, RoleId = r.RoleId, TrackingState = r.TrackingState }).ToList(),
                Permissions = entity.Permissions.Where(p => p.IsGranted).Select(p => new PermissionModel
                {
                    Id = p.Id,
                    TrackingState = p.TrackingState,
                    Name = p.Name
                }).ToList(),
                IgnoredPermissions = entity.Permissions.Where(p => !p.IsGranted).Select(p => new PermissionModel
                {
                    Id = p.Id,
                    TrackingState = p.TrackingState,
                    Name = p.Name
                }).ToList()
            };
        }

        protected override Task BeforeSaveAsync(IReadOnlyList<User> entities, List<UserModel> models)
        {
            ApplyPasswordHash(entities, models);
            ApplySerialNumber(entities, models);
            return base.BeforeSaveAsync(entities, models);
        }

        private void ApplySerialNumber(IEnumerable<User> entities, IReadOnlyList<UserModel> models)
        {
            var i = 0;
            foreach (var entity in entities)
            {
                var model = models[i++];

                if (model.IsNew || !model.IsActive || !model.Password.IsEmpty() ||
                    model.Roles.Any(a => a.IsNew || a.IsDeleted) ||
                    model.IgnoredPermissions.Any(p => p.IsDeleted || p.IsNew) ||
                    model.Permissions.Any(p => p.IsDeleted || p.IsNew))
                {
                    entity.SerialNumber = _manager.NewSerialNumber();
                }
                else
                {
                    //prevent include SerialNumber in update query
                    UnitOfWork.Entry(entity).Property(a => a.SerialNumber).IsModified = false;
                }
            }
        }

        private void ApplyPasswordHash(IEnumerable<User> entities, IReadOnlyList<UserModel> models)
        {
            var i = 0;
            foreach (var entity in entities)
            {
                var model = models[i++];
                if (model.IsNew || !model.Password.IsEmpty())
                {
                    entity.PasswordHash = _manager.HashPassword(model.Password);
                }
                else
                {
                    //prevent include PasswordHash in update query
                    UnitOfWork.Entry(entity).Property(a => a.PasswordHash).IsModified = false;
                }
            }
        }
    }
}