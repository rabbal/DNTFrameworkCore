using System;
using DNTFrameworkCore.Dependency;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Tests
{
    public static class TestingHelper
    {
        public static IServiceProvider BuildServiceProvider(Action<IServiceCollection> configure = null)
        {
            var services = new ServiceCollection();
            
            services.AddDNTFrameworkCore();

            configure?.Invoke(services);

            var serviceProvider = IoC.ApplicationServices = services.BuildServiceProvider();


            return serviceProvider;
        }
    }
}