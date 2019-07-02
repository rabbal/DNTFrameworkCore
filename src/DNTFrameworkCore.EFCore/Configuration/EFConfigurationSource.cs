using System;
using Microsoft.Extensions.Configuration;

namespace DNTFrameworkCore.EFCore.Configuration
{
    public static class ConfigurationBuilderExtensions
    {
        // ReSharper disable once InconsistentNaming
        public static IConfigurationBuilder AddEFCore(this IConfigurationBuilder builder,
            IServiceProvider provider)
        {
            return builder.Add(new EFConfigurationSource(provider));
        }
    }

    // ReSharper disable once InconsistentNaming
    public class EFConfigurationSource : IConfigurationSource
    {
        private readonly IServiceProvider _provider;

        public EFConfigurationSource(IServiceProvider provider)
        {
            _provider = provider;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new EFConfigurationProvider(_provider);
        }
    }
}