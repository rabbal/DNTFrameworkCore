using DNTFrameworkCore.Dependency;
using DNTFrameworkCore.Runtime;

namespace DNTFrameworkCore.Tenancy
{
    public interface ITenantSession : IScopedDependency
    {
        /// <summary>
        ///     Gets current TenantId or null.
        ///     This TenantId should be the TenantId of the <see cref="IUserSession.UserId" />.
        ///     It can be null if given <see cref="IUserSession.UserId" /> is a head-tenant user or no user logged in.
        /// </summary>
        string TenantId { get; }

        /// <summary>
        ///     Gets current TenantName or null.
        ///     This TenantName should be the TenantName of the <see cref="IUserSession.UserId" />.
        ///     It can be null if given <see cref="IUserSession.UserId" /> is a head-tenant user or no user logged in.
        /// </summary>
        string TenantName { get; }

        /// <summary>
        ///     Represents current tenant is head-tenant.
        /// </summary>
        bool IsHeadTenant { get; }

        /// <summary>
        ///     TenantId of the impersonator.
        ///     This is filled if a user with <see cref="IUserSession.ImpersonatorUserId" /> performing actions behalf of the
        ///     <see cref="IUserSession.UserId" />.
        /// </summary>
        string ImpersonatorTenantId { get; }
    }
}