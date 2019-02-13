using System.Text.RegularExpressions;

namespace DNTFrameworkCore.Web.Mvc
{
    public static class UrlChecker
    {
        private static readonly Regex UrlWithProtocolRegex = new Regex("^.{1,10}://.*$");

        public static bool IsRooted(string url)
        {
            return url.StartsWith("/") || UrlWithProtocolRegex.IsMatch(url);
        }
    }
}