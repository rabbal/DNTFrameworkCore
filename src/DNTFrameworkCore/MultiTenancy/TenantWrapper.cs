using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.MultiTenancy
{
    public class TenantWrapper : ITenant, IScopedDependency
    {
        public TenantWrapper(TenantInfo tenant)
        {
            Value = tenant;
        }

        public TenantInfo Value { get; }
    }
}