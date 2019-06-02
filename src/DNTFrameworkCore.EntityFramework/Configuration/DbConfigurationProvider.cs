using System;
using System.Linq;
using DNTFrameworkCore.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EntityFramework.Configuration
{
    //Under development
    public class DbConfigurationProvider : ConfigurationProvider
    {
        private readonly IServiceProvider _serviceProvider;

        public DbConfigurationProvider(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override void Load()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var uow = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                Data?.Clear();
                Data = uow.Set<ConfigurationValue>()
                    .AsNoTracking()
                    .ToList()
                    .ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}