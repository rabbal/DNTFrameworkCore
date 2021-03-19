using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Querying;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectName.Application.Identity;
using ProjectName.Application.Identity.Models;
using ProjectName.Domain.Identity;
using Shouldly;
using static ProjectName.IntegrationTests.TestingHelper;

namespace ProjectName.IntegrationTests.Application
{
    [TestFixture]
    public class UserServiceTests
    {
        private IUserService _service;
        private IServiceProvider _provider;
        private SqliteConnection _connection;

        [SetUp]
        public void Init()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _provider = BuildServiceProvider(DatabaseEngine.SQLite, _connection);

            _service = _provider.GetRequiredService<IUserService>();

            SeedRoles();

            SeedUsers();
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        private void SeedUsers()
        {
            var user1 = new User
            {
                UserName = "User1UserName",
                NormalizedUserName = "User1UserName",
                DisplayName = "user1",
                NormalizedDisplayName = "User1DisplayName",
                PasswordHash = "User1PasswordHash",
                Roles = new List<UserRole>
                {
                    new UserRole {RoleId = 1},
                    new UserRole {RoleId = 2}
                },
                Permissions = new List<UserPermission>
                {
                    new UserPermission
                    {
                        Name = "Permission1",
                        IsGranted = false
                    },
                    new UserPermission
                    {
                        Name = "Permission2",
                        IsGranted = true
                    }
                }
            };

            var user2 = new User
            {
                UserName = "User2UserName",
                NormalizedUserName = "User2UserName",
                DisplayName = "user2",
                NormalizedDisplayName = "User2DisplayName",
                PasswordHash = "User2PasswordHash",
                Roles = new List<UserRole>
                {
                    new UserRole {RoleId = 1}
                },
            };

            _provider.RunScoped<IDbContext>(content =>
            {
                content.SetRowVersionOnInsert(nameof(User));

                content.Set<User>().Add(user1);
                content.Set<User>().Add(user2);
                content.SaveChanges();
            });
        }

        private void SeedRoles()
        {
            var role1 = new Role
            {
                Name = "Role1Name",
                NormalizedName = "Role1Name",
                Permissions = new List<RolePermission>
                {
                    new RolePermission {Name = "Permission1"},
                    new RolePermission {Name = "Permission2"}
                }
            };

            var role2 = new Role
            {
                Name = "Role2Name",
                NormalizedName = "Role2Name",
                Permissions = new List<RolePermission>
                {
                    new RolePermission {Name = "Permission3"},
                    new RolePermission {Name = "Permission4"}
                }
            };

            _provider.RunScoped<IDbContext>(content =>
            {
                content.SetRowVersionOnInsert(nameof(Role));

                content.Set<Role>().Add(role1);
                content.Set<Role>().Add(role2);
                content.SaveChanges();
            });
        }

        [Test]
        public async Task Should_Create_User()
        {
            //Arrange
            var model = new UserModel
            {
                UserName = "NewUserUserName",
                DisplayName = "کاربر جدید",
                Password = "NewUserPassword"
            };

            //Act
            var result = await _service.CreateAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();
            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>()
                    .First(a => a.UserName == model.UserName);
                user.ShouldNotBeNull();
            });
        }

        [Test]
        public async Task Should_Create_User_With_Role()
        {
            //Arrange
            var model = new UserModel
            {
                UserName = "NewUserUserName",
                DisplayName = "کاربر جدید",
                Password = "NewUserPassword",
                Roles = new List<UserRoleModel>
                {
                    new UserRoleModel
                    {
                        RoleId = 1,
                        TrackingState = TrackingState.Added
                    }
                }
            };

            //Act
            var result = await _service.CreateAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();
            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>()
                    .Include(r => r.Roles)
                    .First(a => a.UserName == model.UserName);
                user.ShouldNotBeNull();
                user.Roles.Count.ShouldBe(1);
            });
        }

        [Test]
        public async Task Should_FindById_User()
        {
            //Act
            var user = await _service.FindAsync(1);

            //Assert
            user.HasValue.ShouldBeTrue();
            user.Value.Roles.Count.ShouldBe(2);
            user.Value.Roles.Select(p => p.RoleId).ShouldContain(1);
        }

        [Test]
        public async Task Should_Edit_User()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(2));

            var model = maybe.Value;
            model.UserName = "User2EditedUserName";
            model.DisplayName = "Edited DisplayName";
            model.Password = "User2EditedPassword";

            //Act
            var result = await _service.EditAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>().Include(r => r.Roles).First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.UserName.ShouldBe(model.UserName);
                user.DisplayName.ShouldBe(model.DisplayName);
            });
        }

        [Test]
        public async Task Should_Edit_User_And_Add_New_Role()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(2));

            var model = maybe.Value;
            var role = model.Roles;
            role.Add(new UserRoleModel()
            {
                RoleId = 2,
                TrackingState = TrackingState.Added
            });

            //Act
            var result = await _service.EditAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>().Include(r => r.Roles).First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.Roles.Count.ShouldBe(2);
            });
        }

        [Test]
        public async Task Should_Edit_User_With_Edit_Role()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(2));

            var model = maybe.Value;

            var role = model.Roles.Single(x => x.Id == 3);
            role.RoleId = 2;
            role.TrackingState = TrackingState.Modified;

            //Act
            var result = await
                _provider.RunScoped<Result, IUserService>(service =>
                    service.EditAsync(model));

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>().Include(r => r.Roles).First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.Roles.Any(x => x.RoleId == 2).ShouldBeTrue();
            });
        }

        [Test]
        public async Task Should_Edit_User_And_Delete_Role()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(2));

            var model = maybe.Value;
            model.Roles.Single(x => x.Id == 3).TrackingState = TrackingState.Deleted;

            //Act
            var result = await
                _provider.RunScoped<Result, IUserService>(service =>
                    service.EditAsync(model));

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>().Include(r => r.Roles).First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.Roles.Any(x => x.RoleId == 3).ShouldBeFalse();
            });
        }

        [Test]
        public async Task Should_Reset_SecurityToken_When_Add_New_User()
        {
            //Arrange
            var model = new UserModel
            {
                UserName = "NewUserUserName",
                DisplayName = "کاربر جدید",
                Password = "NewUserPassword"
            };

            var securityToken = _provider.RunScoped<string, IDbContext>(x =>
            {
                var user = x.Set<User>().Find(2L);
                return user.SecurityToken;
            });

            //Act
            var result = await _service.CreateAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>()
                    .First(a => a.UserName == model.UserName);

                user.ShouldNotBeNull();
                user.UserName.ShouldBe(model.UserName);
                user.SecurityToken.ShouldNotBe(securityToken);
            });
        }

        [Test]
        public async Task Should_Reset_SecurityToken_When_Password_Changed()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(2));

            var model = maybe.Value;
            model.Password = "123456";

            var securityToken = _provider.RunScoped<string, IDbContext>(x =>
            {
                var user = x.Set<User>().Find(2L);
                return user.SecurityToken;
            });

            //Act
            var result = await
                _provider.RunScoped<Result, IUserService>(service =>
                    service.EditAsync(model));

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>().First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.SecurityToken.ShouldNotBe(securityToken);
            });
        }

        [Test]
        public async Task Should_Reset_SecurityToken_When_Add_Some_Roles()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(2));

            var model = maybe.Value;

            var securityToken = _provider.RunScoped<string, IDbContext>(x =>
            {
                var user = x.Set<User>().Find(2L);
                return user.SecurityToken;
            });

            var role = model.Roles;
            role.Add(new UserRoleModel
            {
                RoleId = 2,
                TrackingState = TrackingState.Added
            });

            //Act
            var result = await
                _provider.RunScoped<Result, IUserService>(service =>
                    service.EditAsync(model));

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>()
                    .Include(r => r.Roles)
                    .First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.Roles.Count.ShouldBe(2);
                user.SecurityToken.ShouldNotBe(securityToken);
            });
        }

        [Test]
        public async Task Should_Reset_SecurityToken_When_Delete_Some_Roles()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(1));

            var model = maybe.Value;
            model.Roles.First().TrackingState = TrackingState.Deleted;

            var securityToken = _provider.RunScoped<string, IDbContext>(x =>
            {
                var user = x.Set<User>().Find(1L);
                return user.SecurityToken;
            });

            //Act
            var result = await
                _provider.RunScoped<Result, IUserService>(service =>
                    service.EditAsync(model));

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>()
                    .Include(r => r.Roles)
                    .First(a => a.Id == 1);

                user.ShouldNotBeNull();
                user.Roles.Count.ShouldBe(1);
                user.SecurityToken.ShouldNotBe(securityToken);
            });
        }

        [Test]
        public async Task Should_Apply_Password_Hash_When_Add_New_User()
        {
            //Arrange
            var model = new UserModel
            {
                UserName = "NewUserUserName",
                DisplayName = "کاربر جدید",
                Password = "NewUserPassword"
            };

            //Act
            var result = await _service.CreateAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>()
                    .First(a => a.UserName == "NewUserUserName");

                user.ShouldNotBeNull();
                user.PasswordHash.ShouldNotBeNull();
            });
        }

        [Test]
        public async Task Should_Apply_Password_Hash_When_Edit_User_And_Modify_Password()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(2));

            var model = maybe.Value;
            model.Password = "123456";

            var passwordHash = _provider.RunScoped<string, IDbContext>(x =>
            {
                var user = x.Set<User>().Find(2L);
                return user.PasswordHash;
            });

            //Act
            var result = await
                _provider.RunScoped<Result, IUserService>(service =>
                    service.EditAsync(model));

            //Assert
            result.Failed.ShouldBeFalse();

            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>()
                    .First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.PasswordHash.ShouldNotBe(passwordHash);
            });
        }

        [Test]
        public async Task Should_Delete_User_Role()
        {
            //Arrange
            var maybe =
                await _provider.RunScoped<Maybe<UserModel>, IUserService>(service =>
                    service.FindAsync(1));

            //Act
            var model = maybe.Value;
            model.Roles.First().TrackingState = TrackingState.Deleted;

            var result = await
                _provider.RunScoped<Result, IUserService>(service =>
                    service.EditAsync(model));

            //Assert
            result.Failed.ShouldBeFalse();
            _provider.RunScoped<IDbContext>(content =>
            {
                var user = content.Set<User>().Include(x => x.Roles).First(x => x.Id == 1);
                user.Roles.Count.ShouldBe(1);
                user.Roles.First().RoleId.ShouldBe(2);
            });
        }

        [Test]
        public async Task Should_Fetch_Paged_List_Of_Users()
        {
            //Act
            var result = await _service.FetchPagedListAsync(new FilteredPagedRequest
            {
                PageSize = 10,
                Page = 1,
                Sorting = "Id_ASC"
            });

            //Assert
            result.TotalCount.ShouldBe(2);
            result.ItemList.First().UserName.ShouldBe("User1UserName");
            result.ItemList.Last().UserName.ShouldBe("User2UserName");
        }
    }
}