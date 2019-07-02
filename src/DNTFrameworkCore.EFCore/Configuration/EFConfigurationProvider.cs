using System;
using System.Linq;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EFCore.Configuration
{
    // ReSharper disable once InconsistentNaming
    public class EFConfigurationProvider : ConfigurationProvider
    {
        private readonly IServiceProvider _provider;

        public EFConfigurationProvider(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override void Load()
        {
            using (var scope = _provider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                
                Data?.Clear();
                Data = context.Set<ConfigurationValue>()
                    .AsNoTracking()
                    .ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}