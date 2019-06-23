using System;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using DNTFrameworkCore.Numbering;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EFCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNumbering(this IServiceCollection services, Action<NumberingOptions> configuration)
        {
            services.Configure(configuration);

            services.AddTransient<IPreActionHook, PreInsertNumberedEntityHook>();
        }
    }
}