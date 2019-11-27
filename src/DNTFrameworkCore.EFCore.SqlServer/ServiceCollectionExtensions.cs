using System;
using DNTFrameworkCore.EFCore.Context.Hooks;
using DNTFrameworkCore.EFCore.SqlServer.Numbering;
using DNTFrameworkCore.Numbering;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.EFCore.SqlServer
{
    public static class ServiceCollectionExtensions
    {
        public static EFCoreBuilder WithNumberingHook(this EFCoreBuilder builder, Action<NumberingOptions> options)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));
            if (options == null) throw new ArgumentNullException(nameof(options));

            builder.Services.Configure(options);
            builder.Services.AddTransient<IHook, PreInsertNumberedEntityHook>();

            return builder;
        }
    }
}