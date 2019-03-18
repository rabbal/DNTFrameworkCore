using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Extensions
{
    public static class HttpRequestExtensions
    {
        private const string RequestedWithHeader = "X-Requested-With";
        private const string XmlHttpRequest = "XMLHttpRequest";

        /// <summary>
        /// Determines whether the HttpRequest's X-Requested-With header has XMLHttpRequest value.
        /// </summary>
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            return request?.Headers != null && request.Headers[RequestedWithHeader] == XmlHttpRequest;
        }
    }
}