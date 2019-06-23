using System;
using System.Collections.Generic;
using System.Reflection;
using DNTFrameworkCore.Validation;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.FluentValidation
{
    public static class Extensions
    {
        public static IServiceCollection AddFluentModelValidation(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            services.AddTransient(typeof(IModelValidator<>), typeof(FluentValidationModelValidator<>));
            services.AddTransient<IValidatorFactory, ServiceProviderValidatorFactory>();

            return services;
        }

        /// <summary>
        /// Adds all validators in specified assemblies
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="assemblies"></param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssemblies(this IServiceCollection services,
            IEnumerable<Assembly> assemblies, ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            foreach (var assembly in assemblies)
                services.AddValidatorsFromAssembly(assembly, lifetime);

            return services;
        }

        /// <summary>
        /// Adds all validators in specified assembly
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="assembly">The assembly to scan</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssembly(this IServiceCollection services, Assembly assembly,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
        {
            AssemblyScanner
                .FindValidatorsInAssembly(assembly)
                .ForEach(scanResult => services.AddScanResult(scanResult, lifetime));

            return services;
        }

        /// <summary>
        /// Adds all validators in the assembly of the specified type
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="type">The type whose assembly to scan</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssemblyContaining(this IServiceCollection services,
            Type type, ServiceLifetime lifetime = ServiceLifetime.Transient)
            => services.AddValidatorsFromAssembly(type.Assembly, lifetime);

        /// <summary>
        /// Adds all validators in the assembly of the type specified by the generic parameter
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        public static IServiceCollection AddValidatorsFromAssemblyContaining<T>(this IServiceCollection services,
            ServiceLifetime lifetime = ServiceLifetime.Transient)
            => services.AddValidatorsFromAssembly(typeof(T).Assembly, lifetime);

        /// <summary>
        /// Helper method to register a validator from an AssemblyScanner result
        /// </summary>
        /// <param name="services">The collection of services</param>
        /// <param name="scanResult">The scan result</param>
        /// <param name="lifetime">The lifetime of the validators. The default is transient</param>
        /// <returns></returns>
        private static IServiceCollection AddScanResult(this IServiceCollection services,
            AssemblyScanner.AssemblyScanResult scanResult, ServiceLifetime lifetime)
        {
            //Register as interface
            services.Add(
                new ServiceDescriptor(
                    scanResult.InterfaceType,
                    scanResult.ValidatorType,
                    lifetime));

            //Register as self
            services.Add(
                new ServiceDescriptor(
                    scanResult.ValidatorType,
                    scanResult.ValidatorType,
                    lifetime));

            return services;
        }
    }
}