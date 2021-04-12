using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Cqrs.Commands
{
    public interface ICommandHandler<in TCommand> : ICommandHandler<TCommand, Result> where TCommand : ICommand
    {
    }

    public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult> where TCommand : ICommand<TResult> where TResult : Result
    {
    }

    public abstract class CommandHandlerBase<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public abstract Task<Result> Handle(TCommand command, CancellationToken cancellationToken);

        protected static Result Ok() => Result.Ok();
        protected static Result Fail(string message) => Result.Fail(message);
        protected static Result Fail(string message, IEnumerable<ValidationFailure> failures) => Result.Fail(message, failures);
    }

    public abstract class CommandHandlerBase<TCommand, TResult> : ICommandHandler<TCommand, TResult> where TCommand : ICommand<TResult> where TResult : Result
    {
        public abstract Task<TResult> Handle(TCommand command, CancellationToken cancellationToken);

        protected static Result Ok() => Result.Ok();
        protected static Result Fail(string message) => Result.Fail(message);
        protected static Result Fail(string message, IEnumerable<ValidationFailure> failures) => Result.Fail(message, failures);
    }

}
