using System.IO;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EntityFramework.Context;
using DNTFrameworkCore.EntityFramework.SqlServer;
using DNTFrameworkCore.EntityFramework.SqlServer.Numbering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Shouldly;
using static DNTFrameworkCore.EntityFramework.Tests.TestingHelper;

namespace DNTFrameworkCore.EntityFramework.Tests.Numbering
{
    [TestFixture]
    public class NumberingTests
    {
        [Test]
        public void Should_Fill_Number_When_Number_Property_Is_Empty()
        {
            //Arrange
            var provider = BuildServiceProvider();

            //Act, Assert
            provider.RunScoped<IUnitOfWork>(uow =>
            {
                using (var transaction = uow.BeginTransaction())
                {
                    var task = new TestTask();
                    uow.AddRange(new[] {task});
                    uow.SaveChanges();
                    task.Number.ShouldBe("Task-100");

                    transaction.Commit();
                }
            });
        }

        [Test]
        public void Should_Prevent_Modify_Number_On_Update()
        {
            //Arrange
            var provider = BuildServiceProvider();
            provider.RunScoped<IUnitOfWork>(uow =>
            {
                var task = new TestTask {Number = "Task-Number"};
                uow.AddRange(new[] {task});
                uow.SaveChanges();
            });

            //Act
            provider.RunScoped<IUnitOfWork>(uow =>
            {
                var task = new TestTask {Number = "Task-NewNumber"};
                uow.UpdateRange(new[] {task});
                uow.SaveChanges();
            });

            //Assert
            provider.RunScoped<IUnitOfWork>(uow =>
            {
                var task = uow.Set<TestTask>().SingleOrDefault(t=>t.Number=="Task-Number");
                task.ShouldNotBeNull();
            });
        }

        [Test]
        public void
            Should_Fill_Numbers_Without_Gap_When_Concurrent_Transactions_Want_Insert_Instance_Of_INumberedEntity()
        {
            //Arrange
            var provider = BuildServiceProvider();

            //Act
            ExecuteInParallel(delegate
            {
                provider.RunScoped<IUnitOfWork>(uow =>
                {
                    using (var transaction = uow.BeginTransaction())
                    {
                        var task = new TestTask();
                        uow.AddRange(new[] {task});
                        uow.SaveChanges();

                        transaction.Commit();
                    }
                });
            });

            //Assert
            provider.RunScoped<IUnitOfWork>(uow =>
            {
                var tasks = uow.Set<TestTask>().OrderBy(a => a.Id).ToList();
                tasks.Count.ShouldBe(10);
                tasks[0].Number.ShouldBe("Task-100");
                tasks[5].Number.ShouldBe("Task-125");
                tasks[9].Number.ShouldBe("Task-145");
            });
        }

        private static ServiceProvider BuildServiceProvider()
        {
            var services = new ServiceCollection();
            services.AddDNTFramework();
            services.AddDNTUnitOfWork<NumberingDbContext>();
            services.AddNumbering(options =>
            {
                options.NumberedEntityMap[typeof(TestTask)] = new NumberedEntityOption
                {
                    Start = 100,
                    Prefix = "Task-",
                    IncrementBy = 5
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
                                    MultipleActiveResultSets=true;AttachDbFileName={fileName}")
                        .ConfigureWarnings(warnings =>
                        {
                            warnings.Throw(RelationalEventId.QueryClientEvaluationWarning);
                            warnings.Throw(CoreEventId.IncludeIgnoredWarning);
                        }));

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