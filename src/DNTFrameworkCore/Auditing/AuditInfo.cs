using System;

namespace DNTFrameworkCore.Auditing
{
    public class AuditInfo
    {
        public long? TenantId { get; set; }
        public long? UserId { get; set; }
        public long? ImpersonatorUserId { get; set; }
        public long? ImpersonatorTenantId { get; set; }
        public string ServiceName { get; set; }
        public string MethodName { get; set; }
        public string Parameters { get; set; }
        public string ReturnValue { get; set; }
        public DateTimeOffset ExecutionDateTime { get; set; }
        public int ExecutionDuration { get; set; }
        public string UserIp { get; set; }
        public string UserBrowserName { get; set; }

        /// <summary>
        ///     Optional custom data that can be filled and used.
        /// </summary>
        public string ExtensionJson { get; set; }

        /// <summary>
        ///     Exception object, if an exception occurred during execution of the method.
        /// </summary>
        public Exception Exception { get; set; }

        public override string ToString()
        {
            var loggedUserId = UserId.HasValue
                ? "user " + UserId.Value
                : "an anonymous user";

            var message = Exception != null
                ? "exception: " + Exception.Message
                : "succeed";

            return
                $"AUDIT LOG: {ServiceName}.{MethodName} is executed by {loggedUserId} in {ExecutionDuration} ms from {UserIp} IP address with {message}.";
        }
    }
}