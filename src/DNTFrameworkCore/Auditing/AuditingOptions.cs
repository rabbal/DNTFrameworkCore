using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using DNTFrameworkCore.Application.Services;
using DNTFrameworkCore.Helpers;

namespace DNTFrameworkCore.Auditing
{
    public class AuditingOptions
    {
        public bool Enabled { get; set; } = true;
        public bool EnabledForAnonymousUsers { get; set; }

        public bool RunInBackground { get; set; } = false;

        public ISet<Type> IgnoredTypes { get; } = new HashSet<Type>
        {
            typeof(Stream),
            typeof(Expression)
        };

        public bool SaveReturnValues { get; set; } = false;

        public IAuditingSelectorList Selectors { get; } = new AuditingSelectorList
        {
            new NamedTypeSelector(
                "DNTFrameworkCore.ApplicationServices",
                type => typeof(IApplicationService).IsAssignableFrom(type)
            )
        };
    }
}