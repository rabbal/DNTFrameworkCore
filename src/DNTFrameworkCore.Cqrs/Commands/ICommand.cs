using DNTFrameworkCore.Functional;
using MediatR;

namespace DNTFrameworkCore.Cqrs.Commands
{
    public interface ICommand : ICommand<Result>
    {
    }

    public interface ICommand<out TResult> : IRequest<TResult> where TResult : Result
    {
    }
}