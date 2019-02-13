using Microsoft.AspNetCore.Http;

namespace DNTFrameworkCore.Web.Extensions
{
    /// <summary>
    /// More info: http://www.dotnettips.info/post/2518
    /// </summary>
    public static class AjaxExtensions
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