using DNTFrameworkCore.Cqrs.Commands;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace DNTFrameworkCore.Cqrs.Behaviors
{
    class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            throw new NotImplementedException();
        }
    }
}
