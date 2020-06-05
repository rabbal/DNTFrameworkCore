using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Web.Extensions;
using DNTFrameworkCore.Web.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.ExceptionHandling
{
    public static class MvcOptionsExtensions
    {
        public static MvcOptions UseExceptionHandling(this MvcOptions options)
        {
            options.Filters.Add<HandleException>();
            return options;
        }
    }

    public sealed class HandleException : IExceptionFilter
    {
        private readonly ILogger<HandleException> _logger;
        private readonly IOptions<ExceptionOptions> _options;
        private readonly IHostEnvironment _environment;

        public HandleException(ILogger<HandleException> logger, IOptions<ExceptionOptions> options,
            IHostEnvironment environment)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _environment = environment;
        }

        public void OnException(ExceptionContext context)
        {
            if (context.Exception is OperationCanceledException)
            {
                _logger.LogInformation("Request was cancelled");
                context.Result = new StatusCodeResult(400);
            }
            else if (context.Exception is ValidationException validationException)
            {
                _logger.LogInformation($"ValidationException: {context.Exception}");

                context.ModelState.AddValidationException(validationException);

                context.Result = new BadRequestObjectResult(context.ModelState);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            else if (context.Exception is DbConcurrencyException)
            {
                _logger.LogInformation($"DbConcurrencyException: {context.Exception}");

                context.ModelState.AddModelError(string.Empty, _options.Value.DbConcurrencyException);

                context.Result = new BadRequestObjectResult(context.ModelState);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            else if (context.Exception is DbException dbException &&
                     _options.Value.TryFindMapping(dbException, out var mapping))
            {
                _logger.LogInformation($"DbException: {mapping.Message}");

                context.ModelState.AddModelError(mapping.MemberName ?? string.Empty, mapping.Message);

                context.Result = new BadRequestObjectResult(context.ModelState);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            else
            {
                _logger.LogError(new EventId(context.Exception.HResult), context.Exception, context.Exception.Message);

                var traceId = Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;
                var message = context.Exception is DbException
                    ? _options.Value.DbException
                    : _options.Value.InternalServerIssue;

                var result = new Dictionary<string, string>
                {
                    {"traceId", traceId},
                    {"message", message}
                };

                if (_environment.IsDevelopment())
                {
                    result.Add("development_message", context.Exception.ToStringFormat());
                }

                context.Result = new InternalServerErrorObjectResult(result);

                context.HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }

            context.ExceptionHandled = true;
        }
    }
}