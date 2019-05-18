using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;
using System.Collections.Generic;

namespace DNTFrameworkCore.Domain.Services
{
    public abstract class DomainService : IDomainService
    {
        protected static Result Ok() => Result.Ok();
        protected static Result Failed(string message) => Result.Failed(message);
        protected static Result Failed(string message, IEnumerable<ValidationFailure> failures) => Result.Failed(message, failures);
        protected static Result<T> Ok<T>(T value) => Result.Ok(value);
        protected static Result<T> Failed<T>(string message) => Result.Failed<T>(message);
        protected static Result<T> Failed<T>(string message, IEnumerable<ValidationFailure> failures) => Result.Failed<T>(message, failures);
    }
}
