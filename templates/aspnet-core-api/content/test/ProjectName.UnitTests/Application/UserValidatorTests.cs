using System;
using System.Collections.Generic;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Domain;
using DNTFrameworkCore.EFCore.Context;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ProjectName.Application.Identity.Models;
using ProjectName.Application.Identity.Validators;
using ProjectName.Common.Localization;
using ProjectName.Domain.Identity;
using Shouldly;
using static ProjectName.UnitTests.TestingHelper;

namespace ProjectName.UnitTests.Application
{
    [TestFixture]
    public class UserValidatorTests
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
        public void Should_Have_Error_When_DisplayName_Is_Empty()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                DisplayName = null
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.DisplayName.Required");
        }

        [Test]
        public void Should_Have_Error_When_DisplayName_Length_Less_Than_Minimum()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                DisplayName = "a"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.DisplayName.MinimumLength");
        }

        [Test]
        public void Should_Have_Error_When_DisplayName_Length_More_Than_Maximum_Length()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                DisplayName = new string('a', 51)
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.DisplayName.MaximumLength");
        }

        [Test]
        public void Should_Have_Error_When_DisplayName_Is_Not_Unique()
        {
            //Arrange
            _serviceProvider.RunScoped<IDbContext>(context =>
            {
                context.Set<User>().Add(new User {DisplayName = "FirstName", NormalizedDisplayName = User.NormalizeDisplayName("FirstName")});
                context.SaveChanges();
            });

            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);

            var model = new UserModel
            {
                DisplayName = "FirstName"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.DisplayName.Unique");
        }

        [Test]
        public void Should_Not_Have_Error_When_DisplayName_Is_Unique()
        {
            //Arrange
            _serviceProvider.RunScoped<IDbContext>(context =>
            {
                context.Set<User>().Add(new User {DisplayName = "Maryam Mousalou", NormalizedDisplayName = "Maryam Mousalou"});
                context.SaveChanges();
            });

            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                DisplayName = "Maryam Mousalou"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldNotContain(x => x.ErrorMessage == "User.Fields.DisplayName.Unique");
        }

        [Test]
        public void Should_Have_Error_When_UserName_Is_Empty()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                UserName = null
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.UserName.Required");
        }

        [Test]
        public void Should_Have_Error_When_UserName_Length_Less_Than_Minimum()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                UserName = "AB"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.UserName.MinimumLength");
        }

        [Test]
        public void Should_Have_Error_When_UserName_Length_More_Than_Maximum_Length()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                UserName = new string('a', 257)
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.UserName.MaximumLength");
        }

        [Test]
        public void Should_Have_Error_When_UserName_Expression_Is_Not_Regular()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                UserName = "User Name"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.UserName.RegularExpression");
        }

        [Test]
        public void Should_Have_Error_When_UserName_Is_Not_Unique()
        {
            //Arrange
            _serviceProvider.RunScoped<IDbContext>(context =>
            {
                context.Set<User>()
                    .Add(new User {UserName = "UserName", NormalizedUserName = "UserName".ToUpperInvariant()});
                context.SaveChanges();
            });

            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);

            var model = new UserModel
            {
                UserName = "UserName"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.UserName.Unique");
        }

        [Test]
        public void Should_Not_Have_Error_When_UserName_Is_Unique()
        {
            //Arrange
            _serviceProvider.RunScoped<IDbContext>(context =>
            {
                context.Set<User>().Add(new User {UserName = "UserName", NormalizedUserName = "UserName"});
                context.SaveChanges();
            });

            var dbContext = _serviceProvider.GetService<IDbContext>();
            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                UserName = "User"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldNotContain(x => x.ErrorMessage == "User.Fields.UserName.Unique");
        }

        [Test]
        public void Should_Have_Error_When_Password_Is_Empty()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                Password = null
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.Password.Required");
        }

        [Test]
        public void Should_Have_Error_When_Password_Length_Less_Than_Minimum()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                Password = "AB"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.Password.MinimumLength");
        }

        [Test]
        public void Should_Have_Error_When_Password_Length_More_Than_Maximum_Length()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                Password = new string('a', 129)
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.Password.MaximumLength");
        }

        [Test]
        public void Should_Have_Error_When_Roles_Not_Unique()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                DisplayName = "DisplayName",
                UserName = "UserName",
                Roles = new List<UserRoleModel>
                {
                    new UserRoleModel
                    {
                        RoleId = 1
                    },
                    new UserRoleModel
                    {
                        RoleId = 1
                    },
                    new UserRoleModel
                    {
                        RoleId = 2
                    }
                }
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.Roles.Unique");
        }

        [Test]
        public void Should_Not_Have_Error_When_Roles_Not_Unique_But_Exists_In_Deleted_Roles()
        {
            //Arrange
            var dbContext = _serviceProvider.GetService<IDbContext>();

            var validator = new UserValidator(dbContext, _translation);
            var model = new UserModel
            {
                DisplayName = "DisplayName",
                UserName = "UserName",
                Roles = new List<UserRoleModel>
                {
                    new UserRoleModel
                    {
                        Id = 1,
                        RoleId = 1
                    },
                    new UserRoleModel
                    {
                        Id = 2,
                        RoleId = 1,
                        TrackingState = TrackingState.Deleted
                    },
                    new UserRoleModel
                    {
                        Id = 3,
                        RoleId = 2
                    }
                }
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldNotContain(x => x.ErrorMessage == "User.Fields.Roles.Unique");
        }
    }
}