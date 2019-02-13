using System;
using DNTFrameworkCore.EntityFramework.Context.Hooks;
using DNTFrameworkCore.EntityFramework.SqlServer.Numbering;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EntityFramework.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static void AddDNTNumbering(this IServiceCollection services, Action<NumberingOptions> configuration)
        {
            services.Configure(configuration);

            services.AddTransient<IPreActionHook, NumberingPreInsertHook>();
        }
    }
}