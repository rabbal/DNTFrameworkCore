using System.Net;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Web.Results;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Web.Filters
{
    public class HandleExceptionFilter : IExceptionFilter
    {
        private readonly IHostingEnvironment _env;
        private readonly ILogger<HandleExceptionFilter> _logger;

        public HandleExceptionFilter(IHostingEnvironment env, ILogger<HandleExceptionFilter> logger)
        {
            _env = env;
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(new EventId(context.Exception.HResult),
                context.Exception,
                context.Exception.Message);

            var details = context.Exception.ReadExceptionDetails();

            if (context.Exception is ValidationException validationException)
            {
                context.ModelState.AddModelError(string.Empty, validationException.Message);
                foreach (var failure in validationException.Failures)
                {
                    context.ModelState.AddModelError(failure.MemberName, failure.Message);
                }

                context.Result = new BadRequestObjectResult(context.ModelState);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            else if (context.Exception is DbUpdateException && details.Contains("DELETE") &&
                     details.Contains("REFERENCE") &&
                     details.Contains("FK_"))
            {
                context.ModelState.AddModelError(string.Empty,
                    "به دلیل وجود اطلاعات وابسته، امکان حذف وجود ندارد");
                context.Result = new BadRequestObjectResult(context.ModelState);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            else if (context.Exception is ConcurrencyException)
            {
                context.ModelState.AddModelError(string.Empty,
                    "اطلاعات توسط کاربری دیگر در شبکه تغییر کرده است");
                context.Result = new BadRequestObjectResult(context.ModelState);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            }
            else
            {
                string message;
                switch (context.Exception)
                {
                    case DbUpdateException _:
                        message = "امکان ذخیره‌سازی اطلاعات وجود ندارد؛ دوباره تلاش نمائید";
                        break;
                    default:
                        message = "متأسفانه مشکلی در فرآیند انجام درخواست شما پیش آمده است!";
                        break;
                }

                dynamic json = new
                {
                    Message = message
                };

                if (_env.IsDevelopment())
                {
                    json = new
                    {
                        Message = message,
                        DeveloperMessage = context.Exception
                    };
                }

                context.Result = new InternalServerErrorObjectResult(json);
                context.HttpContext.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
            }

            context.ExceptionHandled = true;
        }
    }
}