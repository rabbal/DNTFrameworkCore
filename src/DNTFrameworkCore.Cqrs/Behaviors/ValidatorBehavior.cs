using System;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.Functional;
using MediatR;

namespace DNTFrameworkCore.Cqrs.Behaviors
{
    class ValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand
        where TResponse : Result
    {
        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            //private readonly ILogger<ValidatorBehavior<TRequest, TResponse>> _logger;
            //  private readonly IValidator<TRequest>[] _validators;

            //  public ValidatorBehavior(IValidator<TRequest>[] validators, ILogger<ValidatorBehavior<TRequest, TResponse>> logger)
            //  {
            //      _validators = validators;
            //      _logger = logger;
            //  }

            //  public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
            //  {
            //      var typeName = request.GetGenericTypeName();

            //      _logger.LogInformation("----- Validating command {CommandType}", typeName);

            //      var failures = _validators
            //          .Select(v => v.Validate(request))
            //          .SelectMany(result => result.Errors)
            //          .Where(error => error != null)
            //          .ToList();

            //      if (failures.Any())
            //      {
            //          _logger.LogWarning("Validation errors - {CommandType} - Command: {@Command} - Errors: {@ValidationErrors}", typeName, request, failures);

            //          throw new OrderingDomainException(
            //              $"Command Validation Errors for type {typeof(TRequest).Name}", new ValidationException("Validation exception", failures));
            //      }

            //      return await next();

            throw new NotImplementedException();
        }
    }
}

