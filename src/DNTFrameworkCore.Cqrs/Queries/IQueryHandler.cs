using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace DNTFrameworkCore.Cqrs.Queries
{
    public interface IQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }

    public abstract class QueryHandler<TQuery, TResult> : IQueryHandler<TQuery, TResult> where TQuery : IQuery<TResult>
    {
        public abstract Task<TResult> Handle(TQuery query, CancellationToken cancellationToken);
    }
}