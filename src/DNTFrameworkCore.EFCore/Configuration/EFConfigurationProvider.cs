using System;
using System.Linq;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace DNTFrameworkCore.EFCore.Configuration
{
    public class EFConfigurationProvider : ConfigurationProvider
    {
        private readonly IServiceProvider _provider;

        public EFConfigurationProvider(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public override void Load()
        {
            _provider.RunScoped<IUnitOfWork>(uow =>
            {
                Data?.Clear();
                Data = uow.Set<KeyValue>()
                    .AsNoTracking()
                    .ToDictionary(c => c.Key, c => c.Value);
            });
        }
    }
}