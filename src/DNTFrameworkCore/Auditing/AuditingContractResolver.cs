using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DNTFrameworkCore.Auditing
{
    /// <summary>
    /// Decides which properties of auditing class to be serialized
    /// </summary>
    public class AuditingContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly IEnumerable<Type> _ignoredTypes;

        public AuditingContractResolver(IEnumerable<Type> ignoredTypes)
        {
            _ignoredTypes = ignoredTypes;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            var property = base.CreateProperty(member, memberSerialization);

            if (member.IsDefined(typeof(SkipAuditingAttribute)) || member.IsDefined(typeof(JsonIgnoreAttribute)))
            {
                property.ShouldSerialize = instance => false;
            }

            foreach (var ignoredType in _ignoredTypes)
            {
                if (!ignoredType.GetTypeInfo().IsAssignableFrom(property.PropertyType)) continue;

                property.ShouldSerialize = instance => false;
                break;
            }

            return property;
        }
    }
}