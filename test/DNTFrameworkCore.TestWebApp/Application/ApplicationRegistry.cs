using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Eventing;
using DNTFrameworkCore.FluentValidation;
using DNTFrameworkCore.Transaction.Interception;
using DNTFrameworkCore.Validation.Interception;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestWebApp.Application
{
    public static class ApplicationRegistry
    {
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        public static void AddApplication(this IServiceCollection services)
        {
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
                .AddClasses(classes => classes.AssignableTo(typeof(IDomainEventHandler<>)))
                .AsImplementedInterfaces()
                .WithTransientLifetime());

            services.AddValidatorsFromAssembly(typeof(ApplicationRegistry).Assembly);

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