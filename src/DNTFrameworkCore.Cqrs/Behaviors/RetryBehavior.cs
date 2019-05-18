using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using MediatR;

namespace DNTFrameworkCore.Cqrs.Behaviors
{
    class RetryBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            throw new System.NotImplementedException();
        }
    }
}
