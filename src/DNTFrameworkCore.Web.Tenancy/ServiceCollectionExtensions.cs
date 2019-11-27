using System;
using DNTFrameworkCore.Tenancy;
using DNTFrameworkCore.Web.Tenancy.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Tenancy
{
    public static class ServiceCollectionExtensions
    {
        public static TenantBuilder WithTenantSession(this TenantBuilder builder)
        {
            if (builder == null) throw new ArgumentNullException(nameof(builder));

            builder.Services.AddScoped<ITenantSession, TenantSession>();
            return builder;
        }

//        public static TenantBuilder WithTenantContainer(this TenantBuilder builder)
//        {
//            if (builder == null) throw new ArgumentNullException(nameof(builder));
//
//            builder.Services.AddSingleton<ITenantContainerFactory>(provider =>
//                new TenantContainerFactory(provider, builder.Services));
//            return builder;
//        }
    }
}