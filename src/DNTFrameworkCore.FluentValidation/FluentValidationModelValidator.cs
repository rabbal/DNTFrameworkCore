using System;
using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Validation;
using FluentValidation;

namespace DNTFrameworkCore.FluentValidation
{
    internal class FluentValidationModelValidator<T> : ModelValidator<T>
    {
        private readonly IValidatorFactory _validatorFactory;

        public FluentValidationModelValidator(IValidatorFactory validatorFactory)
        {
            _validatorFactory = validatorFactory ?? throw new ArgumentNullException(nameof(validatorFactory));
        }

        public override IEnumerable<ModelValidationResult> Validate(T model)
        {
            var fvValidator = _validatorFactory.GetValidator(model.GetType());

            if (fvValidator == null) return Enumerable.Empty<ModelValidationResult>();

            var validationResult = fvValidator.Validate(model);
            var failures = validationResult.Errors
                .Select(e => new ModelValidationResult(e.PropertyName, e.ErrorMessage))
                .ToList();

            return failures;
        }
    }
}