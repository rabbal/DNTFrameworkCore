using System;
using DNTFrameworkCore.Auditing;
using DNTFrameworkCore.Domain.Entities;
using DNTFrameworkCore.Exceptions;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.EntityFramework.Auditing
{
    public class AuditLog : Entity
    {
        /// <summary>
        /// Maximum length of <see cref="ServiceName"/> property.
        /// </summary>
        public static int MaxServiceNameLength = 256;

        /// <summary>
        /// Maximum length of <see cref="MethodName"/> property.
        /// </summary>
        public static int MaxMethodNameLength = 256;

        /// <summary>
        /// TenantId.
        /// </summary>
        public long? TenantId { get; set; }

        /// <summary>
        /// UserId.
        /// </summary>
        public long? UserId { get; set; }

        /// <summary>
        /// Service (class/interface) name.
        /// </summary>
        public string ServiceName { get; set; }

        /// <summary>
        /// Executed method name.
        /// </summary>
        public string MethodName { get; set; }

        /// <summary>
        /// Calling parameters.
        /// </summary>
        public string Parameters { get; set; }

        /// <summary>
        /// Return values.
        /// </summary>
        public string ReturnValue { get; set; }

        /// <summary>
        /// Start datetime of the method execution.
        /// </summary>
        public DateTimeOffset ExecutionDateTime { get; set; }

        /// <summary>
        /// Total duration of the method call as milliseconds.
        /// </summary>
        public int ExecutionDuration { get; set; }

        /// <summary>
        /// IP address of the client.
        /// </summary>
        public string UserIp { get; set; }

        /// <summary>
        /// Browser information if this method is called in a web request.
        /// </summary>
        public string UserBrowserName { get; set; }

        /// <summary>
        /// Exception object, if an exception occured during execution of the method.
        /// </summary>
        public string Exception { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.ImpersonatorUserId"/>.
        /// </summary>
        public long? ImpersonatorUserId { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.ImpersonatorTenantId"/>.
        /// </summary>
        public long? ImpersonatorTenantId { get; set; }

        /// <summary>
        /// <see cref="AuditInfo.ExtensionJson"/>.
        /// </summary>
        public string ExtensionJson { get; set; }

        /// <summary>
        /// Creates a new CreateFromAuditInfo from given <see cref="auditInfo"/>.
        /// </summary>
        /// <param name="auditInfo">Source <see cref="AuditInfo"/> object</param>
        /// <returns>The <see cref="AuditLog"/> object that is created using <see cref="auditInfo"/></returns>
        public static AuditLog CreateFromAuditInfo(AuditInfo auditInfo)
        {
            var exception = auditInfo.Exception?.ReadExceptionDetails();

            return new AuditLog
            {
                TenantId = auditInfo.TenantId,
                UserId = auditInfo.UserId,
                ServiceName = auditInfo.ServiceName.TruncateWithPostfix(MaxServiceNameLength),
                MethodName = auditInfo.MethodName.TruncateWithPostfix(MaxMethodNameLength),
                Parameters = auditInfo.Parameters,
                ExecutionDateTime = auditInfo.ExecutionDateTime,
                ExecutionDuration = auditInfo.ExecutionDuration,
                UserIp = auditInfo.UserIp,
                UserBrowserName = auditInfo.UserBrowserName,
                Exception = exception,
                ImpersonatorUserId = auditInfo.ImpersonatorUserId,
                ImpersonatorTenantId = auditInfo.ImpersonatorTenantId,
                ExtensionJson = auditInfo.ExtensionJson,
            };
        }

        public override string ToString()
        {
            return
                $"AUDIT LOG: {ServiceName}.{MethodName} is executed by user {UserId} in {ExecutionDuration} ms from {UserIp} IP address.";
        }
    }
}