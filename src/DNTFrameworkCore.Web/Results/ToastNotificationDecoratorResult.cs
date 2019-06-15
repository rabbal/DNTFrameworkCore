using System.Threading.Tasks;
using DNTFrameworkCore.Web.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace DNTFrameworkCore.Web.Results
{
    public static class ToastNotificationActionResultExtensions
    {
        public static IActionResult WithInformationNotification(this IActionResult result, string message)
        {
            return new ToastNotificationDecoratorResult { InnerResult = result, Type = "info", Message = message };
        }

        public static IActionResult WithErrorNotification(this IActionResult result, string message)
        {
            return new ToastNotificationDecoratorResult { InnerResult = result, Type = "error", Message = message };
        }

        public static IActionResult WithWarningNotification(this IActionResult result, string message)
        {
            return new ToastNotificationDecoratorResult { InnerResult = result, Type = "warning", Message = message };
        }

        public static IActionResult WithSuccessNotification(this IActionResult result, string message)
        {
            return new ToastNotificationDecoratorResult { InnerResult = result, Type = "success", Message = message };
        }
    }
    public class ToastNotificationDecoratorResult : IActionResult
    {
        public IActionResult InnerResult { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }

        public Task ExecuteResultAsync(ActionContext context)
        {
            var factory = context.HttpContext?.RequestServices?.GetRequiredService<ITempDataDictionaryFactory>();
            var tempData = factory?.GetTempData(context.HttpContext);

            tempData.AddToastNotification(Type, Message);
            return InnerResult.ExecuteResultAsync(context);
        }
    }
}