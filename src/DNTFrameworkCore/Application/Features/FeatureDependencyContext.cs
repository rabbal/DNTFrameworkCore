using System;

namespace DNTFrameworkCore.Application.Features
{
    public class FeatureDependencyContext
    {
        public IServiceProvider ServiceProvider { get; }

        public FeatureDependencyContext(IServiceProvider provider)
        {
            ServiceProvider = provider ?? throw new ArgumentNullException(nameof(provider));
        }
    }
}