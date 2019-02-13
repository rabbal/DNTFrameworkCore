using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DNTFrameworkCore.Collections;

namespace DNTFrameworkCore.Validation.Interception
{
    public class DataAnnotationMethodParameterValidator : IMethodParameterValidator
    {
        private readonly IServiceProvider _provider;

        public DataAnnotationMethodParameterValidator(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException();
        }

        public IEnumerable<ModelValidationResult> Validate(object parameter)
        {
            var properties = TypeDescriptor.GetProperties(parameter).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                var validationAttributes = property.Attributes.OfType<ValidationAttribute>().ToArray();
                if (validationAttributes.IsNullOrEmpty())
                {
                    continue;
                }

                var validationContext = new ValidationContext(parameter,
                    _provider,
                    null)
                {
                    DisplayName = property.DisplayName,
                    MemberName = property.Name
                };

                foreach (var attribute in validationAttributes)
                {
                    var failures = new List<ModelValidationResult>();

                    var result = attribute.GetValidationResult(property.GetValue(parameter), validationContext);

                    if (result == ValidationResult.Success) continue;

                    var message = result.ErrorMessage;

                    if (result.MemberNames != null)
                    {
                        foreach (var memberName in result.MemberNames)
                        {
                            var validationResult = new ModelValidationResult(memberName, message);

                            failures.Add(validationResult);
                        }
                    }

                    if (failures.Count == 0)
                    {
                        // result.MemberNames was null or empty.
                        failures.Add(new ModelValidationResult(string.Empty, message));
                    }

                    return failures;
                }
            }

            return Enumerable.Empty<ModelValidationResult>();
        }
    }
}