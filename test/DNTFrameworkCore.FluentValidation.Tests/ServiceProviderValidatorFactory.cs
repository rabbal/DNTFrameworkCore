using System;
using FluentValidation;

namespace DNTFrameworkCore.FluentValidation.Tests
{
    public class ServiceProviderValidatorFactory : ValidatorFactoryBase {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderValidatorFactory(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType) {
            return _serviceProvider.GetService(validatorType) as IValidator;
        }
    }
}