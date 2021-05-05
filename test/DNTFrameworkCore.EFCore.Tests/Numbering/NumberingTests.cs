using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using DNTFrameworkCore.EFCore.SqlServer;
using DNTFrameworkCore.Numbering;
using DNTFrameworkCore.Runtime;
using DNTFrameworkCore.Tenancy;
using DNTFrameworkCore.Timing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using Shouldly;
using static DNTFrameworkCore.EFCore.Tests.TestingHelper;

namespace DNTFrameworkCore.EFCore.Tests.Numbering
{
    [TestFixture]
    public class NumberingTests
    {
        [Test]
        public void Should_Fill_Number_When_Number_Property_Is_Empty()
        {
            //Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var provider = BuildServiceProvider(dateTime);

            //Act, Assert
            provider.RunScoped<IDbContext>(dbContext =>
            {
                dbContext.BeginTransaction();
                var task1 = new NumberingTestEntity
                {
                    DateTime = dateTime,
                    BranchId = 1
                };
                var task2 = new NumberingTestEntity
                {
                    DateTime = dateTime,
                    BranchId = 1
                };
                dbContext.Set<NumberingTestEntity>().Add(task1);
                dbContext.Set<NumberingTestEntity>().Add(task2);
                dbContext.SaveChanges();

                task1.Number.ShouldBe("Prefix-100");

                task2.Number.ShouldBe("Prefix-105");

                dbContext.Set<NumberedEntity>().Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}")
                    .ShouldBeTrue();

                dbContext.CommitTransaction();
            });
        }

        [Test]
        public void Should_Fill_All_NumberingFields_BasedOn_NumberingOptions()
        {
            //Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var provider = BuildServiceProvider(dateTime);

            //Act, Assert
            provider.RunScoped<IDbContext>(dbContext =>
            {
                dbContext.BeginTransaction();
                var task1 = new NumberingTestEntity
                {
                    DateTime = dateTime,
                    BranchId = 1
                };
                var task2 = new NumberingTestEntity
                {
                    DateTime = dateTime,
                    BranchId = 1
                };
                dbContext.Set<NumberingTestEntity>().Add(task1);
                dbContext.Set<NumberingTestEntity>().Add(task2);
                dbContext.SaveChanges();

                task1.Number.ShouldBe("Prefix-100");
                task1.NumberBasedOnBranchId.ShouldBe("10");
                task1.NumberBasedOnBranchIdDateTime.ShouldBe("1");
                task1.NumberBasedOnBranchIdCreatedDateTime.ShouldBe("1");

                task2.Number.ShouldBe("Prefix-105");
                task2.NumberBasedOnBranchId.ShouldBe("20");
                task2.NumberBasedOnBranchIdDateTime.ShouldBe("2");
                task2.NumberBasedOnBranchIdCreatedDateTime.ShouldBe("2");

                dbContext.Set<NumberedEntity>().Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}_BranchId_1")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}_BranchId_1_DateTime_20210101")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName ==
                              $"{typeof(NumberingTestEntity).FullName}_BranchId_1_CreatedDateTime_20210101")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName ==
                              $"{typeof(NumberingTestEntity).FullName}_BranchId_1_CreatedDateTime_20210101" &&
                              _.NextValue == 3)
                    .ShouldBeTrue();
                dbContext.CommitTransaction();
            });
        }

        [Test]
        public void Should_Reset_Number_When_ResetFields_Changed()
        {
            //Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var provider = BuildServiceProvider(dateTime);

            //Act, Assert
            provider.RunScoped<IDbContext>(dbContext =>
            {
                dbContext.BeginTransaction();
                var task1 = new NumberingTestEntity
                {
                    DateTime = dateTime,
                    BranchId = 1
                };
                var task2 = new NumberingTestEntity
                {
                    DateTime = dateTime,
                    BranchId = 2
                };
                dbContext.Set<NumberingTestEntity>().Add(task1);
                dbContext.Set<NumberingTestEntity>().Add(task2);
                dbContext.SaveChanges();

                task1.Number.ShouldBe("Prefix-100");
                task1.NumberBasedOnBranchId.ShouldBe("10");
                task1.NumberBasedOnBranchIdDateTime.ShouldBe("1");
                task1.NumberBasedOnBranchIdCreatedDateTime.ShouldBe("1");

                task2.Number.ShouldBe("Prefix-105");
                task2.NumberBasedOnBranchId.ShouldBe("10");
                task2.NumberBasedOnBranchIdDateTime.ShouldBe("1");
                task2.NumberBasedOnBranchIdCreatedDateTime.ShouldBe("1");

                dbContext.Set<NumberedEntity>().Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}_BranchId_1")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}_BranchId_2")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}_BranchId_1_DateTime_20210101")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName == $"{typeof(NumberingTestEntity).FullName}_BranchId_2_DateTime_20210101")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName ==
                              $"{typeof(NumberingTestEntity).FullName}_BranchId_1_CreatedDateTime_20210101")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName ==
                              $"{typeof(NumberingTestEntity).FullName}_BranchId_2_CreatedDateTime_20210101")
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName ==
                              $"{typeof(NumberingTestEntity).FullName}_BranchId_1_CreatedDateTime_20210101" &&
                              _.NextValue == 2)
                    .ShouldBeTrue();
                dbContext.Set<NumberedEntity>()
                    .Any(_ => _.EntityName ==
                              $"{typeof(NumberingTestEntity).FullName}_BranchId_2_CreatedDateTime_20210101" &&
                              _.NextValue == 2)
                    .ShouldBeTrue();
                dbContext.CommitTransaction();
            });
        }

        [Test]
        public void
            Should_Fill_Numbers_Without_Gap_When_Concurrent_Transactions_Want_Insert_Instance_Of_INumberedEntity()
        {
            //Arrange
            var dateTime = new DateTime(2021, 1, 1);
            var provider = BuildServiceProvider(dateTime);

            //Act
            ExecuteInParallel(delegate
            {
                provider.RunScoped<IDbContext>(dbContext =>
                {
                    dbContext.BeginTransaction();
                    var task = new NumberingTestEntity {DateTime = dateTime};
                    dbContext.Set<NumberingTestEntity>().Add(task);
                    dbContext.SaveChanges();

                    dbContext.CommitTransaction();
                });
            });

            //Assert
            provider.RunScoped<IDbContext>(dbContext =>
            {
                var tasks = dbContext.Set<NumberingTestEntity>().OrderBy(a => a.Id).ToList();
                tasks.Count.ShouldBe(10);
                tasks[0].Number.ShouldBe("Prefix-100");
                tasks[5].Number.ShouldBe("Prefix-125");
                tasks[9].Number.ShouldBe("Prefix-145");
            });
        }

        private static ServiceProvider BuildServiceProvider(DateTime now, long tenantId = 1)
        {
            var services = new ServiceCollection();

            var clock = new Mock<ISystemClock>();
            clock.SetupGet(c => c.Now).Returns(now);
            services.AddSingleton(_ => clock.Object);

            var tenantSession = new Mock<ITenantSession>();
            tenantSession.SetupGet(session => session.TenantId)
                .Returns(tenantId.ToString(CultureInfo.InvariantCulture));
            services.AddScoped(_ => tenantSession.Object);

            var userSession = new Mock<IUserSession>();
            userSession.SetupGet(session => session.UserId).Returns("1");
            userSession.SetupGet(session => session.UserIP).Returns("127.0.0.1");
            userSession.SetupGet(session => session.UserBrowserName).Returns("Numbering Test-Engine");
            services.AddScoped(_ => userSession.Object);

            services.AddEFCore<NumberingDbContext>()
                .WithTenancyHook<long>()
                .WithTrackingHook<long>()
                .WithNumberingHook(options =>
                {
                    options[typeof(NumberingTestEntity)] = new[]
                    {
                        new NumberedEntityOption
                        {
                            FieldName = nameof(NumberingTestEntity.Number),
                            Prefix = "Prefix-",
                            Start = 100,
                            IncrementBy = 5
                        },
                        new NumberedEntityOption
                        {
                            FieldName = nameof(NumberingTestEntity.NumberBasedOnBranchId),
                            Start = 10,
                            IncrementBy = 10,
                            Fields = new[] {nameof(NumberingTestEntity.BranchId)}
                        },
                        new NumberedEntityOption
                        {
                            FieldName = nameof(NumberingTestEntity.NumberBasedOnBranchIdDateTime),
                            Fields = new[]
                            {
                                nameof(NumberingTestEntity.BranchId),
                                nameof(NumberingTestEntity.DateTime)
                            }
                        },
                        new NumberedEntityOption
                        {
                            FieldName = nameof(NumberingTestEntity.NumberBasedOnBranchIdCreatedDateTime),
                            Fields = new[]
                            {
                                nameof(NumberingTestEntity.BranchId),
                                EFCoreShadow.CreatedDateTime
                            }
                        }
                    };
                });

            var fileName =
                Path.Combine(
                    Path.GetDirectoryName(
                        typeof(NumberingTests).GetTypeInfo().Assembly.Location),
                    "NumberingTestDb.mdf");

            services.AddEntityFrameworkSqlServer()
                .AddDbContext<NumberingDbContext>(builder =>
                    builder.UseSqlServer(
                        $@"Data Source=(LocalDB)\MSSQLLocalDb;Initial Catalog=NumberingTestDb;Integrated Security=True;
                                    MultipleActiveResultSets=true;AttachDbFileName={fileName}"));

            var provider = services.BuildServiceProvider();

            provider.RunScoped<NumberingDbContext>(context =>
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            });
            return provider;
        }
    }
}