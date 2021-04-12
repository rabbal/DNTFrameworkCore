using System;
using DNTFrameworkCore.Functional;
using MediatR;

namespace DNTFrameworkCore.Cqrs.Commands
{
    public interface ICommand : ICommand<Result>
    {
    }

    public interface ICommand<out TResult> : IRequest<TResult> where TResult : Result
    {
        Guid Id { get; }
    }

    public abstract class CommandBase : CommandBase<Result>, ICommand
    {
    }

    public abstract class CommandBase<TResult> : ICommand<TResult> where TResult : Result
    {
        public Guid Id { get; }

        protected CommandBase()
        {
            Id = Guid.NewGuid();
        }

        protected CommandBase(Guid id)
        {
            Id = id;
        }
    }
}