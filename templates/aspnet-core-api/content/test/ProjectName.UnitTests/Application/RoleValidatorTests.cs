using System;
using System.Collections.Generic;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using FluentValidation.TestHelper;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectName.Application.Identity.Models;
using ProjectName.Application.Identity.Validators;
using ProjectName.Application.Localization;
using ProjectName.Domain.Identity;
using Shouldly;
using static ProjectName.UnitTests.TestingHelper;

namespace ProjectName.UnitTests.Application
{
    [TestFixture]
    public class RoleValidatorTests
    {
        private ITranslationService _translation;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Init()
        {
            _serviceProvider = PrepareServices();
            _translation = _serviceProvider.GetRequiredService<ITranslationService>();
        }

        [Test]
        public void Should_Have_Error_When_Name_Is_Empty()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Name = null
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "Role.Fields.Name.Required");
        }

        [Test]
        public void Should_Have_Error_When_Name_Length_Less_Than_Minimum()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Name = "AB"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x =>
                x.ErrorMessage == "Role.Fields.Name.MinimumLength");
        }

        [Test]
        public void Should_Have_Error_When_Name_Length_More_Than_Maximum()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Name = new string('A', 51)
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x =>
                x.ErrorMessage == "Role.Fields.Name.MaximumLength");
        }

        [Test]
        public void Should_Have_Error_When_Name_Is_Not_Unique()
        {
            //Arrange
            _serviceProvider.RunScoped<IDbContext>(context =>
            {
                context.Set<Role>()
                    .Add(new Role {Name = "ExistingName", NormalizedName = "ExistingName".ToUpperInvariant()});
                context.SaveChanges();
            });

            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Name = "ExistingName"
            };

            //Act & Assert
            validator.ShouldHaveValidationErrorFor(x => x.Name, model)
                .WithErrorMessage("Role.Fields.Name.Unique");
        }

        [Test]
        public void Should_Not_Have_Error_When_Name_Is_Unique()
        {
            //Arrange
            _serviceProvider.RunScoped<IDbContext>(context =>
            {
                context.Set<Role>().Add(new Role {Name = "ExistingName"});
                context.SaveChanges();
            });

            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Name = "NewName"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldNotContain(x =>
                x.ErrorMessage == "Role.Fields.Name.Unique");
        }

        [Test]
        public void Should_Have_Error_When_Description_Length_More_Than_Maximum()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Description = new string('A', 1025)
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x =>
                x.ErrorMessage == "Role.Fields.Description.MaximumLength");
        }

        [Test]
        public void Should_Have_Error_When_Permissions_Is_Not_Unique()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Name = "Role1",
                Permissions = new List<PermissionModel>
                {
                    new PermissionModel {Name = "Permission1"},
                    new PermissionModel {Name = "Permission1"}
                }
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x =>
                x.ErrorMessage == "Role.Fields.Permissions.Unique");
        }

        [Test]
        public void Should_Not_Have_Error_When_Permissions_Is_Not_Unique_But_Exists_In_Deleted_Permissions()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new RoleValidator(dbContext, _translation);

            var model = new RoleModel
            {
                Name = "Role1",
                Permissions = new List<PermissionModel>
                {
                    new PermissionModel
                    {
                        Id = 1,
                        Name = "Permission1"
                    },
                    new PermissionModel
                    {
                        Id = 2,
                        Name = "Permission1",
                        TrackingState = TrackingState.Deleted
                    }
                }
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldNotContain(x =>
                x.ErrorMessage == "Role.Fields.Permissions.Unique");
        }
    }
}