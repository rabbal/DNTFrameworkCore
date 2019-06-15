using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Validation
{
    public static class ModelValidationResultExtensions
    {
        public static Result ToResult(this IEnumerable<ValidationFailure> results)
        {
            var failures = results as ValidationFailure[] ?? results.ToArray();
            if (!failures.Any()) return Result.Ok();

            var message = string.Join("\n", failures.Where(a => string.IsNullOrEmpty(a.MemberName)));

            var result = Result.Fail(message, failures);

            return result;
        }
    }
}