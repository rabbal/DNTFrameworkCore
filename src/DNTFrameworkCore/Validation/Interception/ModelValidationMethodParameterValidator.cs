using System;
using System.Collections.Generic;
using System.Linq;

namespace DNTFrameworkCore.Validation.Interception
{
    public class ModelValidationMethodParameterValidator : IMethodParameterValidator
    {
        private readonly IServiceProvider _provider;

        public ModelValidationMethodParameterValidator(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IEnumerable<ValidationFailure> Validate(object parameter)
        {
            if (parameter == null)
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            var validatorType = typeof(IModelValidator<>).MakeGenericType(parameter.GetType());

            if (!(_provider.GetService(validatorType) is IModelValidator validator))
                return Enumerable.Empty<ValidationFailure>();

            var failures = validator.Validate(parameter);

            return failures;
        }
    }
}