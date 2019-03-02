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
        Task<Result> Handle(T @event);
    }

    public abstract class BusinessEventHandler<T> : IBusinessEventHandler<T> where T : IBusinessEvent
    {
        public abstract Task<Result> Handle(T @event);
        protected Result Faild(string message) => Result.Failed(message);
        protected Result Faild(string message, IEnumerable<ModelValidationResult> failures) => Result.Failed(message, failures);
        protected Result Ok() => Result.Ok();
    }
}