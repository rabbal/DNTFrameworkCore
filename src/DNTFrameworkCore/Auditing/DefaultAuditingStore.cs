using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.GuardToolkit;
using DNTFrameworkCore.Logging;
using Microsoft.Extensions.Logging;

namespace DNTFrameworkCore.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditingStore"/> to simply write audits to logs.
    /// </summary>
    internal class DefaultAuditingStore : IAuditingStore
    {
        private readonly ILogger<DefaultAuditingStore> _logger;

        public DefaultAuditingStore(ILogger<DefaultAuditingStore> logger)
        {
            Guard.ArgumentNotNull(logger, nameof(logger));

            _logger = logger;
        }

        public void Save(AuditInfo auditInfo)
        {
            if (auditInfo.Exception == null)
            {
                _logger.LogInformation(LoggingEvents.AUDIT_LOG, auditInfo.ToString());
            }
            else
            {
                _logger.LogWarning(LoggingEvents.AUDIT_LOG, auditInfo.Exception, auditInfo.ToString());
            }
        }
    }
}