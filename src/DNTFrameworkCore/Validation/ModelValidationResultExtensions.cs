using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Validation
{
    public static class ModelValidationResultExtensions
    {
        public static Result ToResult(this IEnumerable<ValidationFailure> failures)
        {
            failures = failures as ValidationFailure[] ?? failures.ToArray();
            return !failures.Any() ? Result.Ok() : Result.Fail(string.Empty, failures);
        }
    }
}