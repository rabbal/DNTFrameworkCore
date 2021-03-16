using System;
using System.Text.Json;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Validation;
using DNTFrameworkCore.Web.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.ExceptionHandling
{
    public static class ExceptionHandlingExtensions
    {
        public static ApiBehaviorOptions UseFailureProblemDetailResponseFactory(this ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var detail = FailureProblemDetail.FromHttpContext(context.HttpContext)
                    .WithFailures(context.ModelState.ToSerializable());
                return new BadRequestObjectResult(detail);
            };
            return options;
        }

        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app,
            Action<Exception, ExceptionHandlingContext> handle = null)
        {
            return app.UseExceptionHandler(appException => appException.Run(async context =>
            {
                var feature = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("ExceptionHandling");
                var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
                var options = app.ApplicationServices.GetRequiredService<IOptions<ExceptionOptions>>();

                // Should always exist, but best to be safe!
                if (feature?.Error == null) return;

                var exception = feature.Error;
                var detail = FailureProblemDetail.FromHttpContext(context);

                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                context.Response.ContentType = "application/json";

                var handlingContext = new ExceptionHandlingContext();
                handle?.Invoke(exception, handlingContext);
                if (handlingContext.ExceptionHandled)
                {
                    detail.Message = handlingContext.FriendlyMessage;
                    context.Response.StatusCode = handlingContext.StatusCode;
                }
                else
                {
                    switch (exception)
                    {
                        case OperationCanceledException:
                            logger.LogInformation($"OperationCanceledException: {exception}");

                            detail.Message = "Request was cancelled";
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            break;
                        case ValidationException validationException:
                            logger.LogInformation($"ValidationException: {exception}");

                            detail.Message = validationException.Message;
                            if (validationException.Failures != null)
                                detail.WithFailures(validationException.Failures);
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            break;
                        case DbConcurrencyException:
                            logger.LogInformation($"DbConcurrencyException: {exception}");

                            detail.Message = options.Value.DbConcurrencyException;
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            break;
                        case DbException dbException
                            when options.Value.TryFindMapping(dbException, out var mapping):
                        {
                            logger.LogInformation($"DbException: {mapping.Message}");

                            if (string.IsNullOrEmpty(mapping.MemberName))
                            {
                                detail.Message = mapping.Message;
                            }
                            else
                            {
                                detail.WithFailures(new[]
                                    {new ValidationFailure(mapping.MemberName, mapping.Message)});
                            }

                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            break;
                        }
                        default:
                        {
                            logger.LogError(new EventId(exception.HResult), exception,
                                $"InternalServerIssue: {exception.Message}");

                            detail.Message = exception is DbException
                                ? options.Value.DbException
                                : options.Value.InternalServerIssue;

                            if (env.IsDevelopment())
                            {
                                detail.DevelopmentMessage = exception.ToString();
                            }

                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            break;
                        }
                    }
                }

                var stream = context.Response.Body;
                await JsonSerializer.SerializeAsync(stream, detail);
            }));
        }

        public class ExceptionHandlingContext
        {
            public int StatusCode { get; set; }
            public string FriendlyMessage { get; set; }
            public bool ExceptionHandled { get; set; }
        }
    }
}