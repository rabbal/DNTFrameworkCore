using static ProjectName.IntegrationTests.TestingHelper;

namespace ProjectName.IntegrationTests.Application
{
    [TestFixture]
    public class UserServiceTests
    {
        private IUserService _service;
        private IServiceProvider _serviceProvider;
        private SqliteConnection _connection;

        [SetUp]
        public void Init()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();

            _serviceProvider = BuildServiceProvider(DatabaseEngine.SQLite, _connection);

            _service = _serviceProvider.GetRequiredService<IUserService>();

            SeedRoles();
            Seed();
        }

        [TearDown]
        public void TearDown()
        {
            _connection.Close();
        }

        private void Seed()
        {
            var user1 = new User
            {
                UserName = "User1UserName",
                NormalizedUserName = "User1UserName",
                DisplayName = "User1DisplayName",
                NormalizedDisplayName = "User1DisplayName",
                IsActive = true,
                PasswordHash = "User1PasswordHash",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                Roles = new List<UserRole>
                {
                    new UserRole {RoleId = 1},
                    new UserRole {RoleId = 2}
                }
            };

            var user2 = new User
            {
                UserName = "User2UserName",
                NormalizedUserName = "User2UserName",
                DisplayName = "User2DisplayName",
                NormalizedDisplayName = "User2DisplayName",
                IsActive = true,
                PasswordHash = "User2PasswordHash",
                SecurityStamp = Guid.NewGuid().ToString("N"),
                Roles = new List<UserRole>
                {
                    new UserRole {RoleId = 1}
                }
            };

            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.SetRowVersionOnInsert(nameof(User));

                uow.Set<User>().Add(user1);
                uow.Set<User>().Add(user2);
                uow.SaveChanges();
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

            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.SetRowVersionOnInsert(nameof(Role));

                uow.Set<Role>().Add(role1);
                uow.Set<Role>().Add(role2);
                uow.SaveChanges();
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
                Password = "NewUserPassword",
                Roles = new List<UserRoleModel>
                {
                    new UserRoleModel
                    {
                        RoleId = 1,
                        TrackingState = TrackingState.Added
                    }
                },
                IsActive = true,
            };

            //Act
            var result = await _service.CreateAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                var user = uow.Set<User>()
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
            user.Value.Roles.Select(p => p.RoleId).ShouldContain(2);
        }

        [Test]
        public async Task Should_Edit_User()
        {
            //Arrange
            var editModel =
                await _serviceProvider.RunScoped<Task<Maybe<UserModel>>, IUserService>(service =>
                    service.FindAsync(2));

            var model = editModel.Value;
            model.UserName = "User2EditedUserName";
            model.DisplayName = "نام نمایشی ویرایش شده";
            model.Password = "User2EditedPassword";
            foreach (var permission in model.Roles)
            {
                permission.TrackingState = TrackingState.Deleted;
            }

            model.Roles.AddRange(
                new[]
                {
                    new UserRoleModel {RoleId = 2, TrackingState = TrackingState.Added}
                });

            //Act
            var result = await _service.EditAsync(model);

            //Assert
            result.Failed.ShouldBeFalse();

            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                var user = uow.Set<User>().Include(r => r.Roles).First(a => a.Id == 2);

                user.ShouldNotBeNull();
                user.UserName.ShouldBe(model.UserName);
                user.DisplayName.ShouldBe(model.DisplayName);
                user.Roles.Select(a => a.RoleId).ShouldNotContain(1);
                user.Roles.Count.ShouldBe(1);
                user.Roles.Select(a => a.RoleId).ShouldContain(2);
            });
        }

        [Test]
        public async Task Should_Delete_User()
        {
            //Arrange
            var user =
                await _serviceProvider.RunScoped<Task<Maybe<UserModel>>, IUserService>(service =>
                    service.FindAsync(2));

            //Act
            await _serviceProvider.RunScoped<Task<Result>, IUserService>(service =>
                service.DeleteAsync(user.Value));

            //Assert
            _serviceProvider.RunScoped<IUnitOfWork>(uow =>
            {
                uow.Set<User>().Any(a => a.Id == 2).ShouldBeFalse();
                uow.Set<UserRole>().Any(a => a.UserId == 2).ShouldBeFalse();
            });
        }

        [Test]
        public async Task Should_Read_Paged_List_Of_Users()
        {
            //Act
            var users = await _service.ReadPagedListAsync(new FilteredPagedQueryModel
            {
                PageSize = 10,
                Page = 1,
                SortExpression = "Id_ASC"
            });

            //Assert
            users.TotalCount.ShouldBe(2);
            users.Items.First().UserName.ShouldBe("User1UserName");
            users.Items.Last().UserName.ShouldBe("User2UserName");
        }
    }
}