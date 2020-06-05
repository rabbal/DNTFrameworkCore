using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using DNTFrameworkCore.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.ExceptionHandling
{
    public static class ExceptionHandlingExtensions
    {
        public static IApplicationBuilder UseExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseExceptionHandler(appException => appException.Run(async context =>
            {
                var feature = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;
                var logger = app.ApplicationServices.GetRequiredService<ILoggerFactory>()
                    .CreateLogger("ExceptionHandling");
                var env = app.ApplicationServices.GetRequiredService<IHostEnvironment>();
                var options = app.ApplicationServices.GetRequiredService<IOptions<ExceptionOptions>>();

                // Should always exist, but best to be safe!
                if (feature?.Error != null)
                {
                    logger.LogError(feature.Error, $"InternalServerIssue: {feature.Error.Message}");

                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    context.Response.ContentType = "application/json";

                    var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

                    var result = new Dictionary<string, string>
                    {
                        {"traceId", traceId},
                        {"message", options.Value.InternalServerIssue}
                    };

                    if (env.IsDevelopment())
                    {
                        result.Add("development_message", feature.Error.ToStringFormat());
                    }

                    var stream = context.Response.Body;
                    await JsonSerializer.SerializeAsync(stream, result);
                }
            }));
        }
    }
}