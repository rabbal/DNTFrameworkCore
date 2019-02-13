using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Exceptions
{
    public class ValidationException : FrameworkException
    {
        public IReadOnlyList<ModelValidationResult> Failures { get; }

        public ValidationException(string message) : this(message, Enumerable.Empty<ModelValidationResult>())
        {
        }

        public ValidationException(string message, IEnumerable<ModelValidationResult> failures) : base(message)
        {
            Failures = failures.ToList();
        }
    }
}