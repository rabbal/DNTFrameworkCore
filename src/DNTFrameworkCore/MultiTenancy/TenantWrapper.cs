using System;
using DNTFrameworkCore.Dependency;

namespace DNTFrameworkCore.MultiTenancy
{
    public class TenantWrapper : ITenant, IScopedDependency
    {
        private readonly TenantInfo _tenant;

        public TenantWrapper(TenantInfo tenant)
        {
            _tenant = tenant;
        }

        public TenantInfo Value => _tenant ?? throw new InvalidOperationException();
        public bool HasValue => _tenant != null;
    }
}