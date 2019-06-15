using DNTFrameworkCore.Functional;
using DNTFrameworkCore.Validation;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Cqrs.Commands
{
    public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand, Result> where TCommand : ICommand
    {
    }

    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        public abstract Task<Result> Handle(TCommand command, CancellationToken cancellationToken);

        protected static Result Ok() => Result.Ok();
        protected static Result Fail(string message) => Result.Fail(message);
        protected static Result Fail(string message, IEnumerable<ValidationFailure> failures) => Result.Fail(message, failures);
    }
}
