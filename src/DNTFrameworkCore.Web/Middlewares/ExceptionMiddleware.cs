using System.Text.Json;
using System.Threading.Tasks;
using DNTFrameworkCore.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DNTFrameworkCore.Web.Middlewares
{
    internal class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly IOptions<ExceptionOptions> _options;

        public ExceptionMiddleware(
            RequestDelegate next,
            ILogger<ExceptionMiddleware> logger,
            IWebHostEnvironment env,
            IOptions<ExceptionOptions> options)
        {
            _next = next;
            _logger = logger;
            _env = env;
            _options = options;
        }

        public async Task Invoke(HttpContext context)
        {
            var feature = context.Features[typeof(IExceptionHandlerFeature)] as IExceptionHandlerFeature;

            if (feature?.Error != null)
            {
                _logger.LogError(feature.Error, $"InternalServerIssue: {feature.Error.Message}");

                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";

                if (_env.IsDevelopment())
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        Message = _options.Value.InternalServerIssue,
                        DeveloperMessage = feature.Error.ToStringFormat()
                    }));
                }
                else
                {
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        Message = _options.Value.InternalServerIssue
                    }));
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}