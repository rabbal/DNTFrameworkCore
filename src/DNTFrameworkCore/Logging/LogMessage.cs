using System;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Logging
{
    public struct LogMessage
    {
        public string Message { get; set; }
        public LogLevel Level { get; set; }
        public DateTime CreationTime { get; set; }
        public string LoggerName { get; set; }
        public EventId EventId { get; set; }
        public string UserBrowserName { get; set; }
        public string UserIP { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserDisplayName { get; set; }
        public string TenantId { get; set; }
        public string TenantName { get; set; }
        public string ImpersonatorUserId { get; set; }
        public string ImpersonatorTenantId { get; set; }
    }
}