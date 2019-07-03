using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EFCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static void AddNumbering(this IServiceCollection services)
        {
            services.AddTransient<IPreActionHook, PreInsertNumberedEntityHook>();
        }
    }
}