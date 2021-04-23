using DNTFrameworkCore.Cqrs;
using DNTFrameworkCore.TestCqrsAPI.Application.Catalog.Policies;
using DNTFrameworkCore.TestCqrsAPI.Domain.Catalog.Policies;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.TestCqrsAPI.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCqrs(typeof(DependencyInjection));
            serviceCollection.AddScoped<IPriceTypePolicy, PriceTypePolicy>();
            return serviceCollection;
        }
    }
}