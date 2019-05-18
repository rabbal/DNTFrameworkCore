using DNTFrameworkCore.Cqrs.Behaviors;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Cqrs
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDNTCqrs(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RetryBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidatorBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
        }
    }
}
