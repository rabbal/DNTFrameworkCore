using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Application.Models;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.Functional;
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
    public class RoleServiceTests
    {
        private IRoleService _service;
        private IServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        [SetUp]
        public void Init()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _serviceProvider = BuildServiceProvider(DatabaseEngine.SQLite, _connection);

            _service = _serviceProvider.GetRequiredService<IRoleService>();

            Seed();
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        private void Seed()
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

            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.SetRowVersionOnInsert(nameof(Role));

                uow.Set<Role>().Add(role1);
                uow.Set<Role>().Add(role2);
                uow.SaveChanges();
            });
        }

        [Test]
        public async Task Should_Create_Role()
        {
            //Arrange
            var model = new RoleModel
            {
                Name = "NewRoleName",
                Permissions = new List<PermissionModel>
                {
                    new PermissionModel {Name = "Permission1", TrackingState = TrackingState.Added},
                    new PermissionModel {Name = "Permission2", TrackingState = TrackingState.Added}
                }
            };

            //Act
            var result = await _service.CreateAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                var role = uow.Set<Role>().Include(r => r.Permissions).First(a => a.Name == model.Name);
                role.ShouldNotBeNull();
                role.Permissions.Count.ShouldBe(2);
            });
        }

        [Test]
        public async Task Should_FindById_Role()
        {
            //Act
            var role = await _service.FindAsync(1);

            //Assert
            role.HasValue.ShouldBeTrue();
            role.Value.Permissions.Count.ShouldBe(2);
            role.Value.Permissions.Select(p => p.Name).ShouldContain("Permission1");
            role.Value.Permissions.Select(p => p.Name).ShouldContain("Permission2");
        }

        [Test]
        public async Task Should_Edit_Role()
        {
            //Arrange
            var editModel =
                await _serviceProvider.RunScoped<Task<Maybe<RoleModel>>, IRoleService>(service =>
                    service.FindAsync(2));

            var model = editModel.Value;
            model.Name = "Role2EditedName";
            foreach (var permission in model.Permissions)
            {
                permission.TrackingState = TrackingState.Deleted;
            }

            model.Permissions.AddRange(
                new[]
                {
                    new PermissionModel {Name = "NewPermission1", TrackingState = TrackingState.Added},
                    new PermissionModel {Name = "NewPermission2", TrackingState = TrackingState.Added}
                });

            //Act
            var result = await _service.EditAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();

            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                var role = uow.Set<Role>().Include(r => r.Permissions).First(a => a.Id == 2);

                role.ShouldNotBeNull();
                role.Name.ShouldBe("Role2EditedName");
                role.Permissions.Select(a => a.Name).ShouldNotContain("Permission3");
                role.Permissions.Select(a => a.Name).ShouldNotContain("Permission4");
                role.Permissions.Count.ShouldBe(2);
                role.Permissions.Select(a => a.Name).ShouldContain("NewPermission1");
                role.Permissions.Select(a => a.Name).ShouldContain("NewPermission2");
            });
        }

        [Test]
        public async Task Should_Delete_Role()
        {
            //Arrange
            var role =
                await _serviceProvider.RunScoped<Task<Maybe<RoleModel>>, IRoleService>(service =>
                    service.FindAsync(2));

            //Act
            await _serviceProvider.RunScoped<Task<Result>, IRoleService>(service =>
                service.DeleteAsync(role.Value));

            //Assert
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.Set<Role>().Any(a => a.Id == 2).ShouldBeFalse();
                uow.Set<RolePermission>().Any(a => a.RoleId == 2).ShouldBeFalse();
            });
        }

        [Test]
        public async Task Should_Read_Paged_List_Of_Roles()
        {
            //Act
            var roles = await _service.ReadPagedListAsync(new FilteredPagedQueryModel
            {
                PageSize = 10,
                Page = 1,
                SortExpression = "Id_ASC"
            });

            //Assert
            roles.TotalCount.ShouldBe(2);
            roles.Items.First().Name.ShouldBe("Role1Name");
            roles.Items.Last().Name.ShouldBe("Role2Name");
        }
    }
}