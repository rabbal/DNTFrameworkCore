using System.Collections.Generic;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Domain
{
    public abstract class DomainBusiness : IDomainBusiness
    {
        protected static Result Ok() => Result.Ok();
        protected static Result Fail(string message) => Result.Fail(message);
        protected static Result Fail(string message, IEnumerable<ValidationFailure> failures) => Result.Fail(message, failures);
        protected static Result<T> Ok<T>(T value) => Result.Ok(value);
        protected static Result<T> Fail<T>(string message) => Result.Fail<T>(message);
        protected static Result<T> Fail<T>(string message, IEnumerable<ValidationFailure> failures) => Result.Fail<T>(message, failures);
    }
}
