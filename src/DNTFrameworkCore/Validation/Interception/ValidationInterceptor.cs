using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Castle.DynamicProxy;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Extensions;
using DNTFrameworkCore.Functional;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Threading;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Validation.Interception
{
    /// <summary>
    /// This interceptor is used intercept method calls for classes with methods must be validated.
    /// </summary>
    public sealed class ValidationInterceptor : IInterceptor
    {
        private readonly MethodInvocationValidator _validator;
        private readonly ILogger _logger;

        public ValidationInterceptor(MethodInvocationValidator validator, ILoggerFactory loggerFactory)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _logger = Ensure.IsNotNull(loggerFactory, nameof(loggerFactory))
                .CreateLogger("DNTFrameworkCore.Validation.Interception");
        }

        public void Intercept(IInvocation invocation)
        {
            MethodInfo method;
            try
            {
                method = invocation.MethodInvocationTarget;
            }
            catch
            {
                method = invocation.GetConcreteMethod();
            }

            var failures = _validator.Validate(method, invocation.Arguments);
            var result = failures.ToResult();

            if (!result.Failed)
            {
                _logger.LogInformation(
                    $"Model Validation Completed Successfully: {invocation.TargetType?.FullName}.{method.Name}");

                invocation.Proceed();
                return;
            }

            _logger.LogInformation(
                $"Model Validation Failed: {invocation.TargetType?.FullName}.{method.Name} {result.Failures.Select(f => f.ToString()).PackToString(",")}");

            if (invocation.Method.IsAsync())
            {
                InterceptAsync(invocation, result);
            }
            else
            {
                InterceptSync(invocation, result);
            }
        }

        private static void InterceptAsync(IInvocation invocation, Result result)
        {
            if (invocation.Method.ReturnType == typeof(Task))
            {
                ThrowValidationException(result);
            }
            else //Task<TResult>
            {
                var returnType = invocation.Method.ReturnType.GenericTypeArguments[0];
                if (typeof(Result).IsAssignableFrom(returnType))
                {
                    invocation.ReturnValue = Task.FromResult(result);
                }
                else
                {
                    ThrowValidationException(result);
                }
            }
        }

        private static void InterceptSync(IInvocation invocation, Result result)
        {
            if (invocation.Method.ReturnType == typeof(Result))
            {
                invocation.ReturnValue = result;
            }
            else
            {
                ThrowValidationException(result);
            }
        }

        private static void ThrowValidationException(Result result)
        {
            throw new ValidationException(result.Message, result.Failures);
        }
    }
}