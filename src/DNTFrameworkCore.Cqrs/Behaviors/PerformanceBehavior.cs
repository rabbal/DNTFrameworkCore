using MediatR;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Cqrs.Behaviors
{
    public class PerformanceBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly ILogger _logger;
        public PerformanceBehavior(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("PerformanceBehavior");
        }
        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var stopwatch = Stopwatch.StartNew();
            
            var response = await next();

            stopwatch.Stop();

            _logger.LogInformation($"{request}: {stopwatch.ElapsedMilliseconds} ms");
            
            return response;
        }
    }
}
