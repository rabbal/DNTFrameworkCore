using System.Linq;
using AutoMapper;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Transaction;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.TestAPI.Application.Configuration;
using DNTFrameworkCore.Validation.Interception;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DNTFrameworkCore.FluentValidation;

namespace DNTFrameworkCore.TestAPI.Application
{
    public static class ApplicationRegistry
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static void AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ProjectOptions>(configuration.Bind);

            services.AddAutoMapper(typeof(ApplicationRegistry));
            services.AddValidatorsFromAssembly(typeof(ApplicationRegistry).Assembly);
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
                .WithTransientLifetime());

            foreach (var descriptor in services.Where(s => typeof(IApplicationService).IsAssignableFrom(s.ServiceType))
                .ToList())
            {
                services.Decorate(descriptor.ServiceType, (target, serviceProvider) =>
                    ProxyGenerator.CreateInterfaceProxyWithTargetInterface(
                        descriptor.ServiceType,
                        target, serviceProvider.GetRequiredService<ValidationInterceptor>(),
                        (IInterceptor) serviceProvider.GetRequiredService<TransactionInterceptor>()));
            }
        }
    }
}