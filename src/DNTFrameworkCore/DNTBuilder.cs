using System;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore
{
    public interface IDNTBuilder
    {
        IServiceCollection Services { get; }
    }

    public class DNTBuilder : IDNTBuilder
    {
        public DNTBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }
    }
}