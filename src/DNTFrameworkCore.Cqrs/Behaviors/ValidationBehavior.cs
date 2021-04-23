using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DNTFrameworkCore.Cqrs.Commands;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Functional;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Cqrs.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : ICommand
        where TResponse : Result
    {
        private readonly ILogger<ValidationBehavior<TRequest, TResponse>> _logger;
        private readonly IValidator<TRequest>[] _validators;

        public ValidationBehavior(IValidator<TRequest>[] validators,
            ILogger<ValidationBehavior<TRequest, TResponse>> logger)
        {
            _validators = validators;
            _logger = logger;
        }

        public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken,
            RequestHandlerDelegate<TResponse> next)
        {
            var typeName = request.GetGenericTypeName();

            _logger.LogInformation("----- Validating command {CommandType}", typeName);

            var failures = _validators
                .Select(v => v.Validate(request))
                .SelectMany(result => result.Errors)
                .Where(error => error != null)
                .ToList();

            if (failures.Any())
            {
                _logger.LogInformation(
                    "Validation Failures - {CommandType} - Command: {@Command} - Errors: {@Failures}", typeName,
                    request, failures);

                throw new ValidationException($"Command Validation Exception for type {typeof(TRequest).Name}",
                    failures);
            }

            return next();
        }
    }
}