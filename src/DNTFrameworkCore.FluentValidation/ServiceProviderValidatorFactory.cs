using System;
using DNTFrameworkCore.Dependency;
using FluentValidation;

namespace DNTFrameworkCore.FluentValidation
{
    /// <summary>
    /// Validator factory implementation that uses the asp.net service provider to construct validators.
    /// </summary>
    internal class ServiceProviderValidatorFactory : ValidatorFactoryBase, ITransientDependency
    {
        private readonly IServiceProvider _provider;

        public ServiceProviderValidatorFactory(IServiceProvider provider)
        {
            _provider = provider;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _provider.GetService(validatorType) as IValidator;
        }
    }
}