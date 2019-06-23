using System.Collections.Generic;
using System.Threading.Tasks;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;

namespace DNTFrameworkCore.Eventing
{
    public interface IBusinessEventHandler<in T> : ITransientDependency
        where T : IBusinessEvent
    {
        Task<Result> Handle(T businessEvent);
    }

    public abstract class BusinessEventHandler<T> : IBusinessEventHandler<T> where T : IBusinessEvent
    {
        public abstract Task<Result> Handle(T businessEvent);
        protected Result Fail(string message) => Result.Fail(message);

        protected Result Fail(string message, IEnumerable<ValidationFailure> failures) =>
            Result.Fail(message, failures);

        protected Result Ok() => Result.Ok();
    }
}