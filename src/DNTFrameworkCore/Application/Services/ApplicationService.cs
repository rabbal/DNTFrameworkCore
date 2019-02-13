using DNTFrameworkCore.Functional;

namespace DNTFrameworkCore.Application.Services
{
    /// <summary>
    /// base class of application services for support versioning
    /// </summary>
    public abstract class ApplicationService : IApplicationService
    {
        protected static Result Ok() => Result.Ok();

        protected static Result Failed(string message) => Result.Failed(message);

        protected static Result<T> Ok<T>(T value) => Result.Ok(value);

        protected static Result<T> Failed<T>(string message) => Result.Failed<T>(message);
    }
}