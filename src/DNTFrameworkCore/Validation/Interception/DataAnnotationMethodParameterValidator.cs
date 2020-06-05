using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DNTFrameworkCore.Collections;

namespace DNTFrameworkCore.Validation.Interception
{
    internal sealed class DataAnnotationMethodParameterValidator : IMethodParameterValidator
    {
        private readonly IServiceProvider _provider;

        public DataAnnotationMethodParameterValidator(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException();
        }

        public IEnumerable<ValidationFailure> Validate(object validatingObject)
        {
            var failures = new List<ValidationFailure>();

            var properties = TypeDescriptor.GetProperties(validatingObject).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                var attributes = property.Attributes.OfType<ValidationAttribute>().ToArray();
                if (attributes.IsNullOrEmpty())
                {
                    continue;
                }

                var context = new ValidationContext(validatingObject, _provider, null)
                {
                    DisplayName = property.DisplayName,
                    MemberName = property.Name
                };

                foreach (var attribute in attributes)
                {
                    var result = attribute.GetValidationResult(property.GetValue(validatingObject), context);

                    if (result == null) continue;

                    if (!result.MemberNames.IsNullOrEmpty())
                    {
                        failures.AddRange(result.MemberNames.Select(memberName =>
                            new ValidationFailure(memberName, result.ErrorMessage)));
                    }
                    else
                    {
                        failures.Add(new ValidationFailure(string.Empty, result.ErrorMessage));
                    }
                }
            }

            return failures;
        }
    }
}