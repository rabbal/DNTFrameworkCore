using System;
using System.Linq;
using DNTFrameworkCore.Configuration;
using DNTFrameworkCore.EFCore.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EFCore.Configuration
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
                var context = scope.ServiceProvider.GetRequiredService<IDbContext>();

                Data?.Clear();
                Data = context.Set<ConfigurationValue>()
                    .AsNoTracking()
                    .ToList()
                    .ToDictionary(c => c.Key, c => c.Value);
            }
        }
    }
}