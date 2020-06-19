using System;
using System.Collections.Generic;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Exceptions
{
    [Serializable]
    public class BusinessRuleException : ValidationException
    {
        public BusinessRuleException(string message) : base(message)
        {
        }

        public BusinessRuleException(string message, IEnumerable<ValidationFailure> failures) : base(message, failures)
        {
        }
    }
}