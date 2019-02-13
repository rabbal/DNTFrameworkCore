using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DNTFrameworkCore.Collections;
using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Runtime;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace DNTFrameworkCore.Auditing
{
    public interface IAuditingHelper : IScopedDependency
    {
        bool ShouldAudit(MethodInfo methodInfo, bool defaultValue = false);

        AuditInfo BuildAuditInfo(Type type, MethodInfo method, object[] arguments);

        AuditInfo BuildAuditInfo(Type type, MethodInfo method, IDictionary<string, object> arguments);

        void Save(AuditInfo auditInfo);
    }

    internal class AuditingHelper : IAuditingHelper
    {
        private readonly IAuditingStore _store;
        private readonly IOptions<AuditingOptions> _options;
        private readonly IUserSession _session;
        private readonly ILogger _logger;

        public AuditingHelper(
            IAuditingStore store,
            IOptions<AuditingOptions> options,
            IUserSession session,
            ILogger<AuditingHelper> logger)
        {
            _store = store ?? throw new ArgumentNullException(nameof(store));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _session = session ?? throw new ArgumentNullException(nameof(session));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public bool ShouldAudit(MethodInfo methodInfo, bool defaultValue = false)
        {
            if (!_options.Value.Enabled)
            {
                return false;
            }

            if (!_options.Value.EnabledForAnonymousUsers && _session?.UserId == null)
            {
                return false;
            }

            if (methodInfo == null)
            {
                return false;
            }

            if (!methodInfo.IsPublic)
            {
                return false;
            }

            if (methodInfo.IsDefined(typeof(EnableAuditingAttribute), true))
            {
                return true;
            }

            if (methodInfo.IsDefined(typeof(SkipAuditingAttribute), true))
            {
                return false;
            }

            var classType = methodInfo.DeclaringType;
            if (classType == null) return defaultValue;

            if (classType.GetTypeInfo().IsDefined(typeof(EnableAuditingAttribute), true))
            {
                return true;
            }

            if (classType.GetTypeInfo().IsDefined(typeof(SkipAuditingAttribute), true))
            {
                return false;
            }

            return _options.Value.Selectors.Any(selector => selector.Predicate(classType)) || defaultValue;
        }

        public AuditInfo BuildAuditInfo(Type type, MethodInfo method, object[] arguments)
        {
            return BuildAuditInfo(type, method, CreateArgumentsDictionary(method, arguments));
        }

        public AuditInfo BuildAuditInfo(Type type, MethodInfo method, IDictionary<string, object> arguments)
        {
            var auditInfo = new AuditInfo
            {
                TenantId = _session.TenantId,
                UserId = _session.UserId,
                ImpersonatorUserId = _session.ImpersonatorUserId,
                ImpersonatorTenantId = _session.ImpersonatorTenantId,
                ServiceName = type != null
                    ? type.FullName
                    : "",
                MethodName = method.Name,
                Parameters = ConvertArgumentsToJson(arguments),
                ExecutionDateTime = DateTimeOffset.UtcNow,
                UserBrowserName = _session.UserBrowserName,
                UserIp = _session.UserIP
            };

            return auditInfo;
        }

        public void Save(AuditInfo auditInfo)
        {
            _store.Save(auditInfo);
        }

        private string ConvertArgumentsToJson(IDictionary<string, object> arguments)
        {
            try
            {
                if (arguments.IsNullOrEmpty())
                {
                    return "{}";
                }

                var dictionary = new Dictionary<string, object>();

                foreach (var argument in arguments)
                {
                    if (argument.Value != null &&
                        _options.Value.IgnoredTypes.Any(t => t.IsInstanceOfType(argument.Value)))
                    {
                        dictionary[argument.Key] = null;
                    }
                    else
                    {
                        dictionary[argument.Key] = argument.Value;
                    }
                }

                return Serialize(dictionary);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex.ToString(), ex);
                return "{}";
            }
        }

        private string Serialize(Dictionary<string, object> dictionary)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new AuditingContractResolver(_options.Value.IgnoredTypes)
            };

            return JsonConvert.SerializeObject(dictionary, options);
        }

        private static Dictionary<string, object> CreateArgumentsDictionary(MethodBase method,
            IReadOnlyList<object> arguments)
        {
            var parameters = method.GetParameters();
            var dictionary = new Dictionary<string, object>();

            for (var i = 0; i < parameters.Length; i++)
            {
                dictionary[parameters[i].Name] = arguments[i];
            }

            return dictionary;
        }
    }
}