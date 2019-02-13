using DNTFrameworkCore.MultiTenancy;

namespace DNTFrameworkCore.Web.MultiTenancy
{
    public class TenantPipelineBuilderContext
    {
        public TenantContext TenantContext { get; set; }
        public TenantInfo Tenant { get; set; }
    }
}