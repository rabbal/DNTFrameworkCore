using MediatR;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
