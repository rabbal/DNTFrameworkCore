using System;
using System.Collections.Generic;

namespace DNTFrameworkCore.TestTenancy.Tenancy
{
    public class TenantOptions
    {
        public IList<TenantOption> Tenants { get; } = new List<TenantOption>();
    }
    
    public class TenantOption
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ISet<string> Hostnames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    }
}