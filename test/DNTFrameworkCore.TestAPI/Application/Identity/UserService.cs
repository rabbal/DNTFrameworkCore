using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Cryptography;
using DNTFrameworkCore.EFCore.Application;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.TestAPI.Application.Identity.Models;
using DNTFrameworkCore.TestAPI.Domain.Identity;
using Microsoft.EntityFrameworkCore;

namespace DNTFrameworkCore.TestAPI.Application.Identity
{
    public interface IUserService : ICrudService<long, UserReadModel, UserModel>
    {
    }

    public class UserService : CrudService<User, long, UserReadModel, UserModel>, IUserService
    {
        private readonly IMapper _mapper;
        private readonly IUserPasswordHashAlgorithm _password;

        public UserService(
            IUnitOfWork uow,
            IEventBus bus,
            IUserPasswordHashAlgorithm password,
            IMapper mapper) : base(uow, bus)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _password = password ?? throw new ArgumentNullException(nameof(password));
        }

        protected override IQueryable<User> BuildFindQuery()
        {
            return base.BuildFindQuery()
                .Include(u => u.Roles)
                .Include(u => u.Permissions);
        }

        protected override IQueryable<UserReadModel> BuildReadQuery(FilteredPagedQueryModel model)
        {
            return EntitySet.AsNoTracking()
                .Select(u => new UserReadModel
                {
                    Id = u.Id,
                    IsActive = u.IsActive,
                    UserName = u.UserName,
                    DisplayName = u.DisplayName,
                    LastLoggedInDateTime = u.LastLoggedInDateTime
                });
        }

        protected override Task<Result> BeforeEditAsync(IReadOnlyList<ModifiedModel<UserModel>> models, IReadOnlyList<User> entities)
        {
            return base.BeforeEditAsync(models, entities);
        }

        protected override void MapToEntity(UserModel model, User user)
        {
            _mapper.Map(model, user);
            
            MapSerialNumber(user, model);
            MapPasswordHash(user, model);
        }

        protected override UserModel MapToModel(User user)
        {
            return _mapper.Map<UserModel>(user);
        }

        private void MapSerialNumber(User user, UserModel model)
        {
            if (!model.ShouldMapSerialNumber()) return;

            user.SerialNumber = User.NewSerialNumber();
        }

        private void MapPasswordHash(User user, UserModel model)
        {
            if (!model.ShouldMapPasswordHash()) return;

            user.PasswordHash = _password.HashPassword(model.Password);
        }
    }
}