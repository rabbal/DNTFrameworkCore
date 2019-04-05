using System.Linq;
using AutoMapper;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EntityFramework.SqlServer;
using DNTFrameworkCore.EntityFramework.SqlServer.Numbering;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.TestWebApp.Application.Configuration;
using DNTFrameworkCore.TestWebApp.Domain.Catalog;
using DNTFrameworkCore.TestWebApp.Domain.Invoices;
using DNTFrameworkCore.TestWebApp.Domain.Tasks;
using DNTFrameworkCore.Transaction.Interception;
using DNTFrameworkCore.Validation;
using DNTFrameworkCore.Validation.Interception;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestWebApp.Application
{
    public static class ApplicationRegistry
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ProjectSetting>(configuration.Bind);

            services.AddAutoMapper();
            services.AddDNTNumbering(options =>
            {
                options.NumberedEntityMap[typeof(Task)] = new NumberedEntityOption
                {
                    Start = 100,
                    Prefix = "Task_",
                    IncrementBy = 5
                };
                options.NumberedEntityMap[typeof(Product)] = new NumberedEntityOption
                {
                    Start = 1,
                    Prefix = "P_",
                    IncrementBy = 1
                };
                options.NumberedEntityMap[typeof(Invoice)] = new NumberedEntityOption
                {
                    Start = 1,
                    IncrementBy = 5
                };
            });

            services.Scan(scan => scan
                .FromCallingAssembly()
                .AddClasses(classes => classes.AssignableTo<ISingletonDependency>())
                .AsMatchingInterface()
                .WithSingletonLifetime()
                .AddClasses(classes => classes.AssignableTo<IScopedDependency>())
                .AsMatchingInterface()
                .WithScopedLifetime()
                .AddClasses(classes => classes.AssignableTo<ITransientDependency>())
                .AsMatchingInterface()
                .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IBusinessEventHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IModelValidator<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime()
                .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            foreach (var descriptor in services.Where(s => typeof(IApplicationService).IsAssignableFrom(s.ServiceType))
                .ToList())
            {
                services.Decorate(descriptor.ServiceType, (target, serviceProvider) =>
                    ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
                        descriptor.ServiceType,
                        target, serviceProvider.GetRequiredService<ValidationInterceptor>(),
                        (IInterceptor)serviceProvider.GetRequiredService<TransactionInterceptor>()));
            }
        }
    }
}