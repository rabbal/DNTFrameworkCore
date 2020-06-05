using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using DNTFrameworkCore.Collections;

namespace DNTFrameworkCore.Validation.Interception
{
    internal sealed class ValidatableObjectMethodParameterValidator : IMethodParameterValidator
    {
        public IEnumerable<ValidationFailure> Validate(object validatingObject)
        {
            if (!(validatingObject is IValidatableObject validatable))
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            var results = validatable.Validate(new ValidationContext(validatingObject));
            var failures = new List<ValidationFailure>();

            foreach (var result in results)
            {
                if (result == ValidationResult.Success) continue;

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

            return failures;
        }
    }
}