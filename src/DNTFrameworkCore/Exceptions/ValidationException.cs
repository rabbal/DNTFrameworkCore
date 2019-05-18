using System.Collections.Generic;
using System.Linq;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Exceptions
{
    public class ValidationException : FrameworkException
    {
        public IReadOnlyList<ValidationFailure> Failures { get; }

        public ValidationException(string message) : this(message, Enumerable.Empty<ValidationFailure>())
        {
        }

        public ValidationException(string message, IEnumerable<ValidationFailure> failures) : base(message)
        {
            Failures = failures.ToList();
        }
    }
}