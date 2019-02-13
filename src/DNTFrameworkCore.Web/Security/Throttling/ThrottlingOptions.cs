namespace DNTFrameworkCore.Web.Security.Throttling
{
    /// <summary>
    /// AntiDos Config
    /// </summary>
    public class ThrottlingOptions
    {
        /// <summary>
        /// Such as looking for `etc/passwd` in the requested URL.
        /// </summary>
        public string[] UrlAttackVectors { set; get; }

        /// <summary>
        /// Such as `google` or `bing`.
        /// </summary>
        public string[] GoodBotsUserAgents { set; get; }

        /// <summary>
        /// Such as `asafaweb`.
        /// </summary>
        public string[] BadBotsUserAgents { set; get; }

        /// <summary>
        /// Such as `HTTP_ACUNETIX_PRODUCT`.
        /// </summary>
        public string[] BadBotsRequestHeaders { set; get; }

        /// <summary>
        /// List of the permanent banned IPs.
        /// </summary>
        public string[] BannedIPAddressRanges { set; get; }

        /// <summary>
        /// How long a client should be banned in minutes?
        /// </summary>
        public int DurationMin { set; get; }

        /// <summary>
        /// Number of allowed requests per `DurationMin`.
        /// </summary>
        public int AllowedRequests { set; get; }

        /// <summary>
        /// An HTML error message for the banned users.
        /// </summary>
        public string ErrorMessage { set; get; }

        /// <summary>
        /// Should we apply this middleware to the localhost requests?
        /// </summary>
        public bool IgnoreLocalHost { set; get; }
    }
}