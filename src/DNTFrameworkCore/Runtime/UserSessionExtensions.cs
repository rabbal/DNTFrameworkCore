using DNTFrameworkCore.Exceptions;

namespace DNTFrameworkCore.Runtime
{
    public static class UserSessionExtensions
    {
      /// <summary>
        /// Gets current User's Id.
        /// Throws <see cref="FrameworkException"/> if <see cref="IUserSession.UserId"/> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current User's Id.</returns>
        public static long GetUserId(this IUserSession session)
        {
            if (!session.UserId.HasValue)
            {
                throw new FrameworkException("Session.UserId is null! Probably, user is not logged in.");
            }

            return session.UserId.Value;
        }

        /// <summary>
        /// Gets current Tenant's Id.
        /// Throws <see cref="FrameworkException"/> if <see cref="IUserSession.TenantId"/> is null.
        /// </summary>
        /// <param name="session">Session object.</param>
        /// <returns>Current Tenant's Id.</returns>
        /// <exception cref="FrameworkException"></exception>
        public static long GetTenantId(this IUserSession session)
        {
            if (!session.TenantId.HasValue)
            {
                throw new FrameworkException("Session.TenantId is null! Possible problems: No user logged in or current logged in user in a host user (TenantId is always null for host users).");
            }

            return session.TenantId.Value;
        }
    }
}