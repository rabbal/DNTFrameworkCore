using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DNTFrameworkCore.Collections;
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
{  [TestFixture]
    public class RoleServiceTests
    {
        private IRoleService _service;
        private IServiceProvider _provider;
        private SqliteConnection _connection;

        [SetUp]
        public void Init()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _provider = BuildServiceProvider(DatabaseEngine.SQLite, _connection);

            _service = _provider.GetRequiredService<IRoleService>();

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

            var administrator = new Role
            {
                Name = RoleNames.Administrators,
                NormalizedName = "Admin",
                Permissions = new List<RolePermission>
                 {
                     new RolePermission {Name = "Permission1"},
                     new RolePermission {Name = "Permission2"},
                     new RolePermission {Name = "Permission3"},
                     new RolePermission {Name = "Permission4"}
                 }
            };

            _provider.RunScoped<IDbContext>(content =>
            {
                content.SetRowVersionOnInsert(nameof(Role));

                content.Set<Role>().Add(role1);
                content.Set<Role>().Add(role2);
                content.Set<Role>().Add(administrator);
                content.SaveChanges();
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
            _provider.RunScoped<IDbContext>(content =>
            {
                var role = content.Set<Role>().Include(r => r.Permissions).First(a => a.Name == model.Name);
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
            var maybe = await _provider.RunScoped<Maybe<RoleModel>, IRoleService>(service => service.FindAsync(2));

            var model = maybe.Value;
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

            _provider.RunScoped<IDbContext>(content =>
            {
                var role = content.Set<Role>().Include(r => r.Permissions).First(a => a.Id == 2);

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
            //Act
            var result = await _provider.RunScoped<Result, IRoleService>(service => service.DeleteAsync(2));

            //Assert
            result.Failed.ShouldBeFalse();
            _provider.RunScoped<IDbContext>(content =>
            {
                content.Set<Role>().Any(a => a.Id == 2).ShouldBeFalse();
                content.Set<RolePermission>().Any(a => a.RoleId == 2).ShouldBeFalse();
            });
        }

        [Test]
        public async Task Should_Fetch_Paged_List_Of_Roles()
        {
            //Act
            var result = await _service.FetchPagedListAsync(new FilteredPagedRequest
            {
                PageSize = 10,
                Page = 1,
                Sorting = "Id_ASC"
            });

            //Assert
            result.TotalCount.ShouldBe(3);
            result.ItemList.First().Name.ShouldBe("Role1Name");
            result.ItemList.Last().Name.ShouldBe(RoleNames.Administrators);
        }
    }
}