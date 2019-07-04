using System;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using DNTFrameworkCore.Numbering;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EFCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNumbering(this IServiceCollection services, Action<NumberingOptions> setupAction)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (setupAction == null) throw new ArgumentNullException(nameof(setupAction));

            services.Configure(setupAction);
            services.AddTransient<IPreActionHook, PreInsertNumberedEntityHook>();
        }
    }
}