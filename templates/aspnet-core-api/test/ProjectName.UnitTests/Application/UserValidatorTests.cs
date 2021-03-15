using static ProjectName.UnitTests.TestingHelper;

namespace ProjectName.UnitTests.Application
{
    [TestFixture]
    public class UserValidatorTests
    {
        private IMessageLocalizer _localizer;
        private IServiceProvider _serviceProvider;

        [SetUp]
        public void Init()
        {
            _serviceProvider = BuildServiceProvider();
            _localizer = _serviceProvider.GetRequiredService<IMessageLocalizer>();
        }

        [Test]
        public void Should_Have_Error_When_DisplayName_Is_Empty()
        {
            //Arrange
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
            var model = new UserModel
            {
                DisplayName = "مس"
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
            var model = new UserModel
            {
                DisplayName = new string('م', 51)
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.DisplayName.MaximumLength");
        }

        [Test]
        public void Should_Have_Error_When_DisplayName_Expression_Is_Not_Regular()
        {
            //Arrange
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
            var model = new UserModel
            {
                DisplayName = "AbCd123"
            };

            //Act
            var result = validator.Validate(model);

            //Assert
            result.Errors.ShouldContain(x => x.ErrorMessage == "User.Fields.DisplayName.RegularExpression");
        }

        [Test]
        public void Should_Have_Error_When_DisplayName_Is_Not_Unique()
        {
            //Arrange
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.Set<User>().Add(new User {DisplayName = "نام", NormalizedDisplayName = "نام".ToUpperInvariant()});
                uow.SaveChanges();
            });

            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);

            var model = new UserModel
            {
                DisplayName = "نام"
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
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.Set<User>().Add(new User {DisplayName = "مریم موسالو", NormalizedDisplayName = "مریم موسالو"});
                uow.SaveChanges();
            });

            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            var validator = new UserValidator(unitOfWork, _localizer);
            var model = new UserModel
            {
                DisplayName = "مریم"
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
            var model = new UserModel
            {
                UserName = "سام تکست"
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
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.Set<User>()
                    .Add(new User {UserName = "UserName", NormalizedUserName = "UserName".ToUpperInvariant()});
                uow.SaveChanges();
            });

            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);

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
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.Set<User>().Add(new User {UserName = "UserName", NormalizedUserName = "UserName"});
                uow.SaveChanges();
            });

            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();
            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
            var model = new UserModel
            {
                DisplayName = "نام",
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
            var unitOfWork = _serviceProvider.GetService<IUnitOfWork>();

            var validator = new UserValidator(unitOfWork, _localizer);
            var model = new UserModel
            {
                DisplayName = "نام",
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