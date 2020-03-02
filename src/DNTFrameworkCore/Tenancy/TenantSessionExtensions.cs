using System;
using DNTFrameworkCore.Extensions;

namespace DNTFrameworkCore.Tenancy
{
    public static class TenantSessionExtensions
    {
        public static T TenantId<T>(this ITenantSession session)
        {
            if (string.IsNullOrEmpty(session.TenantId))
                throw new InvalidOperationException("This ITenantSession.TenantId is Null or Empty");
            
            return session.TenantId.To<T>();
        }

        public static T ImpersonatorTenantId<T>(this ITenantSession session)
        {
            if (string.IsNullOrEmpty(session.ImpersonatorTenantId))
                throw new InvalidOperationException("This ITenantSession.ImpersonatorTenantId is Null or Empty");
            
            return session.ImpersonatorTenantId.To<T>();
        }
    }
}