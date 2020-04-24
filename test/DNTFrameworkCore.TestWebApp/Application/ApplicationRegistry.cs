using System.Linq;
using AutoMapper;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Transaction;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.Mapping;
using DNTFrameworkCore.TestWebApp.Application.Configuration;
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
            services.Configure<ProjectOptions>(configuration.Bind);

            services.AddAutoMapper(typeof(ApplicationRegistry));
            services.AddValidatorsFromAssemblyContaining(typeof(ApplicationRegistry));

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
                        serviceProvider.GetRequiredService<TransactionInterceptor>()));
            }
        }
    }
}